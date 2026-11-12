using System.Net;
using System.Threading;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PuntoIOS.Api.Controllers;
using PuntoIOS.Application.Common.Pagination;
using PuntoIOS.Application.Features.Auditoria;
using PuntoIOS.Application.Features.Clientes;
using PuntoIOS.Application.Features.Dispositivos;
using PuntoIOS.Application.Features.Reparaciones;
using PuntoIOS.Domain.Auditoria;
using PuntoIOS.Domain.Identidad;
using PuntoIOS.Infrastructure.Persistence;

namespace PuntoIOS.IntegrationTests;

public class EntregarOrdenEndpointTests(PostgresFixture postgres) : IClassFixture<PostgresFixture>
{
    private const string Ruta = "/api/v1/reparaciones";

    private static int _contadorDeClientes;

    // Cada llamada crea un cliente con un teléfono distinto: este helper se usa varias veces dentro del mismo test
    // (p. ej. la matriz de autorización crea varias órdenes), y un teléfono repetido en el mismo local haría que
    // CrearClienteHandler devuelva "teléfono duplicado" en lugar del cliente, rompiendo todo lo que sigue.
    private static async Task<OrdenReparacionDto> OrdenAsync(HttpClient admin)
    {
        var telefono = $"3498 3333{Interlocked.Increment(ref _contadorDeClientes):D2}";
        var cliente = (await (await admin.PostAsJsonAsync("/api/v1/clientes", new CrearClienteRequest("Ana", "Pérez", telefono, null, null, null)))
            .Content.ReadFromJsonAsync<ClienteDto>())!;
        var equipo = (await (await admin.PostAsJsonAsync(
                $"/api/v1/clientes/{cliente.Id}/dispositivos", new CrearDispositivoRequest("Apple", "iPhone 13", null, null, null, null)))
            .Content.ReadFromJsonAsync<DispositivoDto>())!;
        var creada = await admin.PostAsJsonAsync(
            Ruta,
            new RecibirEquipoRequest(cliente.Id, equipo.Id, null, "No carga", ["conector_carga"], null, null, null, "pin", "246810"));
        Assert.Equal(HttpStatusCode.Created, creada.StatusCode);
        return (await creada.Content.ReadFromJsonAsync<OrdenReparacionDto>())!;
    }

    private static async Task LlevarAsync(HttpClient cliente, Guid ordenId, params string[] estados)
    {
        foreach (var estado in estados)
        {
            var respuesta = await cliente.PostAsJsonAsync($"{Ruta}/{ordenId}/estado", new CambiarEstadoRequest(estado));
            Assert.Equal(HttpStatusCode.OK, respuesta.StatusCode);
        }
    }

    private static async Task ResponderPresupuestoAsync(HttpClient admin, Guid ordenId, bool aprobado)
    {
        var respuesta = await admin.PostAsJsonAsync($"{Ruta}/{ordenId}/presupuestos/respuesta", new ResponderPresupuestoRequest(aprobado));
        Assert.Equal(HttpStatusCode.OK, respuesta.StatusCode);
    }

    /// <summary>
    /// Lleva la orden por el flujo real hasta "esperando_aprobacion": cambio genérico hasta esperando presupuesto →
    /// se crea la versión 1 (queda en presupuesto enviado) → cambio genérico a esperando aprobación.
    /// </summary>
    private static async Task LlevarAEsperandoAprobacionAsync(HttpClient admin, Guid ordenId)
    {
        await LlevarAsync(admin, ordenId, "en_pruebas", "en_diagnostico", "esperando_presupuesto");
        var presupuesto = await admin.PostAsJsonAsync(
            $"{Ruta}/{ordenId}/presupuestos", new CrearVersionPresupuestoRequest([new("Conector de carga", "repuesto", 8000m)]));
        Assert.Equal(HttpStatusCode.OK, presupuesto.StatusCode);
        await LlevarAsync(admin, ordenId, "esperando_aprobacion");
    }

    /// <summary>Orden nueva llevada por el flujo real hasta uno de los tres estados desde los que se puede entregar.</summary>
    private static async Task<OrdenReparacionDto> OrdenEnAsync(HttpClient admin, string origen)
    {
        var orden = await OrdenAsync(admin);
        switch (origen)
        {
            case "listo_para_retirar":
                await LlevarAEsperandoAprobacionAsync(admin, orden.Id);
                await ResponderPresupuestoAsync(admin, orden.Id, aprobado: true);
                await LlevarAsync(admin, orden.Id, "en_reparacion", "reparacion_completada", "listo_para_retirar");
                break;
            case "presupuesto_rechazado":
                await LlevarAEsperandoAprobacionAsync(admin, orden.Id);
                await ResponderPresupuestoAsync(admin, orden.Id, aprobado: false);
                break;
            case "irreparable":
                await LlevarAsync(admin, orden.Id, "en_pruebas", "en_diagnostico", "irreparable");
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(origen), origen, null);
        }

        Assert.Equal(origen, (await admin.GetFromJsonAsync<OrdenReparacionDto>($"{Ruta}/{orden.Id}"))!.Estado);
        return orden;
    }

    private static Task<HttpResponseMessage> EntregarAsync(
        HttpClient cliente, Guid ordenId, string? garantiaCodigo = "sin_garantia", decimal montoCobrado = 0m, string? metodoPago = "efectivo") =>
        cliente.PostAsJsonAsync($"{Ruta}/{ordenId}/entrega", new EntregarOrdenRequest(garantiaCodigo, montoCobrado, metodoPago));

    private static async Task<string?> CodigoAsync(HttpResponseMessage respuesta) =>
        (await respuesta.Content.ReadFromJsonAsync<JsonElement>()).GetProperty("codigo").GetString();

    private static void AssertEntrega(OrdenReparacionDto orden, string garantia, int? dias, decimal monto, string metodo)
    {
        Assert.Equal("entregado", orden.Estado);
        Assert.Empty(orden.EstadosDisponibles);
        Assert.Null(orden.DesbloqueoTipo);
        Assert.Equal(garantia, orden.GarantiaCodigo);
        Assert.Equal(dias, orden.GarantiaDias);
        Assert.NotNull(orden.EntregadoEn);
        Assert.Equal(
            dias is null ? (DateOnly?)null : DateOnly.FromDateTime(orden.EntregadoEn.Value.UtcDateTime).AddDays(dias.Value),
            orden.GarantiaVenceEn);
        Assert.Equal(monto, orden.MontoCobrado);
        Assert.Equal(metodo, orden.MetodoPago);
    }

    [Theory]
    [InlineData("listo_para_retirar")]
    [InlineData("presupuesto_rechazado")]
    [InlineData("irreparable")]
    public async Task Entregar_registra_cobro_y_garantia_pasa_a_entregado_y_queda_en_el_historial(string origen)
    {
        using var api = await TestApi.CrearAsync(postgres);
        var admin = api.ClienteAdministrador();
        var orden = await OrdenEnAsync(admin, origen);

        var respuesta = await EntregarAsync(admin, orden.Id, "30_dias", 18500.75m, "tarjeta_debito");

        Assert.Equal(HttpStatusCode.OK, respuesta.StatusCode);
        var entregada = (await respuesta.Content.ReadFromJsonAsync<OrdenReparacionDto>())!;
        AssertEntrega(entregada, "30_dias", 30, 18500.75m, "tarjeta_debito");

        var leida = (await admin.GetFromJsonAsync<OrdenReparacionDto>($"{Ruta}/{orden.Id}"))!;
        AssertEntrega(leida, "30_dias", 30, 18500.75m, "tarjeta_debito");
        Assert.Equal(entregada.GarantiaVenceEn, leida.GarantiaVenceEn);

        await using (var scope = api.Servicios.CreateAsyncScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<PuntoIosDbContext>();
            var guardada = await db.OrdenesReparacion.AsNoTracking().SingleAsync(o => o.Id == orden.Id);
            Assert.Null(guardada.DesbloqueoCifrado);
        }

        var historial = (await admin.GetFromJsonAsync<PagedResult<RegistroHistorialDto>>($"{Ruta}/{orden.Id}/historial"))!;
        var entrega = Assert.Single(historial.Items, h => h.Accion == AccionesAuditadas.EntregaRegistrada);
        Assert.Equal(
            [
                new CambioDeCampo("garantia", null, "30 días"),
                new CambioDeCampo("monto_cobrado", null, "18500.75"),
                new CambioDeCampo("metodo_pago", null, "Tarjeta de débito"),
            ],
            entrega.Cambios);
        Assert.Contains(
            historial.Items,
            h => h.Accion == AccionesAuditadas.EstadoCambiado
                && h.Cambios.SequenceEqual(new[] { new CambioDeCampo("estado", origen, "entregado") }));
    }

    [Fact]
    public async Task Entregar_sin_garantia_deja_dias_y_vencimiento_vacios()
    {
        using var api = await TestApi.CrearAsync(postgres);
        var admin = api.ClienteAdministrador();
        var orden = await OrdenEnAsync(admin, "irreparable");

        var respuesta = await EntregarAsync(admin, orden.Id, "sin_garantia", 0m, "efectivo");

        Assert.Equal(HttpStatusCode.OK, respuesta.StatusCode);
        AssertEntrega((await respuesta.Content.ReadFromJsonAsync<OrdenReparacionDto>())!, "sin_garantia", null, 0m, "efectivo");
        AssertEntrega((await admin.GetFromJsonAsync<OrdenReparacionDto>($"{Ruta}/{orden.Id}"))!, "sin_garantia", null, 0m, "efectivo");
    }

    [Fact]
    public async Task Datos_de_entrega_invalidos_devuelven_400_sin_tocar_la_orden()
    {
        using var api = await TestApi.CrearAsync(postgres);
        var admin = api.ClienteAdministrador();
        var orden = await OrdenEnAsync(admin, "listo_para_retirar");

        foreach (var (garantia, monto, metodo) in new (string?, decimal, string?)[]
                 {
                     ("90_dias", 1000m, "efectivo"),
                     (null, 1000m, "efectivo"),
                     ("30_dias", 1000m, "cheque"),
                     ("30_dias", 1000m, null),
                     ("30_dias", -1m, "efectivo"),
                     ("30_dias", 10.001m, "efectivo"),
                     ("30_dias", 100_000_000m, "efectivo"),
                 })
        {
            var invalida = await EntregarAsync(admin, orden.Id, garantia, monto, metodo);
            Assert.Equal(HttpStatusCode.BadRequest, invalida.StatusCode);
            Assert.Equal("entrega_invalida", await CodigoAsync(invalida));
        }

        var sinCambios = (await admin.GetFromJsonAsync<OrdenReparacionDto>($"{Ruta}/{orden.Id}"))!;
        Assert.Equal("listo_para_retirar", sinCambios.Estado);
        Assert.Equal("pin", sinCambios.DesbloqueoTipo);
        Assert.Null(sinCambios.GarantiaCodigo);
        Assert.Null(sinCambios.GarantiaDias);
        Assert.Null(sinCambios.GarantiaVenceEn);
        Assert.Null(sinCambios.MontoCobrado);
        Assert.Null(sinCambios.MetodoPago);
        Assert.Null(sinCambios.EntregadoEn);

        var historial = (await admin.GetFromJsonAsync<PagedResult<RegistroHistorialDto>>($"{Ruta}/{orden.Id}/historial"))!;
        Assert.DoesNotContain(historial.Items, h => h.Accion == AccionesAuditadas.EntregaRegistrada);
    }

    [Fact]
    public async Task Una_orden_fuera_de_un_estado_entregable_o_inexistente_no_se_puede_entregar()
    {
        using var api = await TestApi.CrearAsync(postgres);
        var admin = api.ClienteAdministrador();
        var recibida = await OrdenAsync(admin);

        var fueraDeTurno = await EntregarAsync(admin, recibida.Id);
        Assert.Equal(HttpStatusCode.Conflict, fueraDeTurno.StatusCode);
        Assert.Equal("fuera_de_estado_entregable", await CodigoAsync(fueraDeTurno));
        var sinCambios = (await admin.GetFromJsonAsync<OrdenReparacionDto>($"{Ruta}/{recibida.Id}"))!;
        Assert.Equal("recibido", sinCambios.Estado);
        Assert.Null(sinCambios.EntregadoEn);

        // Una orden ya entregada tampoco se vuelve a entregar.
        var entregada = await OrdenEnAsync(admin, "irreparable");
        Assert.Equal(HttpStatusCode.OK, (await EntregarAsync(admin, entregada.Id, "15_dias", 500m, "efectivo")).StatusCode);
        var otraVez = await EntregarAsync(admin, entregada.Id, "60_dias", 9000m, "tarjeta_credito");
        Assert.Equal(HttpStatusCode.Conflict, otraVez.StatusCode);
        Assert.Equal("fuera_de_estado_entregable", await CodigoAsync(otraVez));
        AssertEntrega((await admin.GetFromJsonAsync<OrdenReparacionDto>($"{Ruta}/{entregada.Id}"))!, "15_dias", 15, 500m, "efectivo");

        var inexistente = await EntregarAsync(admin, Guid.NewGuid());
        Assert.Equal(HttpStatusCode.NotFound, inexistente.StatusCode);
        Assert.Equal("orden_inexistente", await CodigoAsync(inexistente));
    }

    [Fact]
    public async Task El_cambio_de_estado_generico_a_entregado_se_rechaza_desde_cualquier_estado()
    {
        using var api = await TestApi.CrearAsync(postgres);
        var admin = api.ClienteAdministrador();

        // Desde "recibido" la tabla ni siquiera lo permitiría; desde "irreparable" sí. En los dos casos: usar la entrega.
        var recibida = await OrdenAsync(admin);
        var irreparable = await OrdenEnAsync(admin, "irreparable");
        // Ni el detalle lo ofrece: desde "irreparable" la tabla solo lleva a "entregado", que va por la entrega.
        Assert.Empty((await admin.GetFromJsonAsync<OrdenReparacionDto>($"{Ruta}/{irreparable.Id}"))!.EstadosDisponibles);

        foreach (var (orden, estado) in new[] { (recibida, "recibido"), (irreparable, "irreparable") })
        {
            var respuesta = await admin.PostAsJsonAsync($"{Ruta}/{orden.Id}/estado", new CambiarEstadoRequest("entregado"));
            Assert.Equal(HttpStatusCode.Conflict, respuesta.StatusCode);
            Assert.Equal("usar_entrega", await CodigoAsync(respuesta));
            Assert.Equal(estado, (await admin.GetFromJsonAsync<OrdenReparacionDto>($"{Ruta}/{orden.Id}"))!.Estado);
        }
    }

    [Fact]
    public async Task Matriz_de_autorizacion_para_entregar()
    {
        using var api = await TestApi.CrearAsync(postgres);
        var admin = api.ClienteAdministrador();
        var recepcion = api.Cliente(await api.RegistrarUsuarioAsync("recepcion@x.com", RolesDeSistema.RecepcionVentasId));
        var tecnico = api.Cliente(await api.RegistrarUsuarioAsync("tecnico@x.com", RolesDeSistema.TecnicoId));
        var anonimo = api.Cliente();

        var soloConsultaId = Guid.NewGuid();
        await using (var scope = api.Servicios.CreateAsyncScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<PuntoIosDbContext>();
            var rol = new Rol(soloConsultaId, "Solo consulta", "Solo ve las órdenes de reparación", false);
            rol.Conceder(Permisos.ReparacionesVer);
            db.Roles.Add(rol);
            await db.SaveChangesAsync();
        }

        var soloConsulta = api.Cliente(await api.RegistrarUsuarioAsync("consulta@x.com", soloConsultaId));

        // Cobrar al entregar es de Recepción/Ventas (y del administrador): el técnico NO tiene el permiso.
        var orden = await OrdenEnAsync(admin, "irreparable");
        Assert.Equal(HttpStatusCode.Forbidden, (await EntregarAsync(tecnico, orden.Id)).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await EntregarAsync(soloConsulta, orden.Id)).StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, (await EntregarAsync(anonimo, orden.Id)).StatusCode);
        Assert.Equal("irreparable", (await admin.GetFromJsonAsync<OrdenReparacionDto>($"{Ruta}/{orden.Id}"))!.Estado);

        foreach (var cliente in new[] { recepcion, admin })
        {
            var otra = await OrdenEnAsync(admin, "irreparable");
            var respuesta = await EntregarAsync(cliente, otra.Id, "45_dias", 12000m, "transferencia_bancaria");
            Assert.Equal(HttpStatusCode.OK, respuesta.StatusCode);
            AssertEntrega((await respuesta.Content.ReadFromJsonAsync<OrdenReparacionDto>())!, "45_dias", 45, 12000m, "transferencia_bancaria");
        }
    }
}
