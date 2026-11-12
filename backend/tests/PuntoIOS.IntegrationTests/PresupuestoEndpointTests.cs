using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
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

public class PresupuestoEndpointTests(PostgresFixture postgres) : IClassFixture<PostgresFixture>
{
    private const string Ruta = "/api/v1/reparaciones";

    private static readonly string[] HastaEsperandoPresupuesto = ["en_pruebas", "en_diagnostico", "esperando_presupuesto"];

    private static async Task<OrdenReparacionDto> OrdenAsync(HttpClient admin)
    {
        var cliente = (await (await admin.PostAsJsonAsync("/api/v1/clientes", new CrearClienteRequest("Ana", "Pérez", "3498 111111", null, null, null)))
            .Content.ReadFromJsonAsync<ClienteDto>())!;
        var equipo = (await (await admin.PostAsJsonAsync(
                $"/api/v1/clientes/{cliente.Id}/dispositivos", new CrearDispositivoRequest("Apple", "iPhone 13", null, null, null, null)))
            .Content.ReadFromJsonAsync<DispositivoDto>())!;
        var creada = await admin.PostAsJsonAsync(
            Ruta, new RecibirEquipoRequest(cliente.Id, equipo.Id, null, "Pantalla rota", ["pantalla"]));
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

    private static Task<HttpResponseMessage> PresupuestarAsync(HttpClient cliente, Guid ordenId, List<ItemPresupuestoRequest>? items) =>
        cliente.PostAsJsonAsync($"{Ruta}/{ordenId}/presupuestos", new CrearVersionPresupuestoRequest(items));

    /// <summary>Registra la respuesta al presupuesto vigente: es la única forma de pasar a "aprobado" o "presupuesto_rechazado".</summary>
    private static async Task ResponderPresupuestoAsync(HttpClient cliente, Guid ordenId, bool aprobado)
    {
        var respuesta = await cliente.PostAsJsonAsync($"{Ruta}/{ordenId}/presupuestos/respuesta", new ResponderPresupuestoRequest(aprobado));
        Assert.Equal(HttpStatusCode.OK, respuesta.StatusCode);
    }

    /// <summary>Entrega el equipo (sin garantía ni cobro): es la única forma de pasar a "entregado".</summary>
    private static async Task EntregarAsync(HttpClient cliente, Guid ordenId)
    {
        var respuesta = await cliente.PostAsJsonAsync($"{Ruta}/{ordenId}/entrega", new EntregarOrdenRequest("sin_garantia", 0m, "efectivo"));
        Assert.Equal(HttpStatusCode.OK, respuesta.StatusCode);
    }

    private static List<ItemPresupuestoRequest> ItemsValidos() =>
    [
        new("Pantalla OLED", "repuesto", 15000m),
        new("Cambio de pantalla", "mano_obra", 2500.50m),
    ];

    private static async Task<string?> CodigoAsync(HttpResponseMessage respuesta) =>
        (await respuesta.Content.ReadFromJsonAsync<JsonElement>()).GetProperty("codigo").GetString();

    [Fact]
    public async Task Crear_un_presupuesto_desde_esperando_presupuesto_lo_deja_vigente_y_pasa_a_presupuesto_enviado()
    {
        using var api = await TestApi.CrearAsync(postgres);
        var admin = api.ClienteAdministrador();
        var orden = await OrdenAsync(admin);
        Assert.Null(orden.PresupuestoVigente);
        await LlevarAsync(admin, orden.Id, HastaEsperandoPresupuesto);

        var respuesta = await PresupuestarAsync(admin, orden.Id, ItemsValidos());

        Assert.Equal(HttpStatusCode.OK, respuesta.StatusCode);
        var actualizada = (await respuesta.Content.ReadFromJsonAsync<OrdenReparacionDto>())!;
        Assert.Equal("presupuesto_enviado", actualizada.Estado);
        Assert.Equal(1, actualizada.PresupuestoVigente!.Numero);
        Assert.Equal(17500.50m, actualizada.PresupuestoVigente.Total);

        var leida = (await admin.GetFromJsonAsync<OrdenReparacionDto>($"{Ruta}/{orden.Id}"))!;
        Assert.Equal("presupuesto_enviado", leida.Estado);
        Assert.Equal(17500.50m, leida.PresupuestoVigente!.Total);
        Assert.Equal(
            [new ItemPresupuestoDto("Pantalla OLED", "repuesto", 15000m), new ItemPresupuestoDto("Cambio de pantalla", "mano_obra", 2500.50m)],
            leida.PresupuestoVigente.Items);

        var historial = (await admin.GetFromJsonAsync<PagedResult<RegistroHistorialDto>>($"{Ruta}/{orden.Id}/historial"))!;
        var creado = Assert.Single(historial.Items, h => h.Accion == AccionesAuditadas.PresupuestoCreado);
        Assert.Contains(new CambioDeCampo("total_presupuesto", null, "17500.50"), creado.Cambios);
        Assert.Contains(
            historial.Items,
            h => h.Accion == AccionesAuditadas.EstadoCambiado
                && h.Cambios.SequenceEqual(new[] { new CambioDeCampo("estado", "esperando_presupuesto", "presupuesto_enviado") }));
    }

    [Fact]
    public async Task Listar_devuelve_las_versiones_de_la_mas_nueva_a_la_mas_vieja_con_sus_items_y_total()
    {
        using var api = await TestApi.CrearAsync(postgres);
        var admin = api.ClienteAdministrador();
        var orden = await OrdenAsync(admin);
        await LlevarAsync(admin, orden.Id, HastaEsperandoPresupuesto);

        Assert.Empty((await admin.GetFromJsonAsync<List<VersionDePresupuestoDto>>($"{Ruta}/{orden.Id}/presupuestos"))!);
        Assert.Equal(HttpStatusCode.OK, (await PresupuestarAsync(admin, orden.Id, ItemsValidos())).StatusCode);
        Assert.Equal(
            HttpStatusCode.OK,
            (await PresupuestarAsync(admin, orden.Id, [new("Pantalla original", "repuesto", 30000m)])).StatusCode);

        var versiones = (await admin.GetFromJsonAsync<List<VersionDePresupuestoDto>>($"{Ruta}/{orden.Id}/presupuestos"))!;

        Assert.Equal([2, 1], versiones.Select(v => v.Numero));
        Assert.Equal(30000m, versiones[0].Total);
        Assert.Equal(new ItemPresupuestoDto("Pantalla original", "repuesto", 30000m), Assert.Single(versiones[0].Items));
        Assert.Equal(17500.50m, versiones[1].Total);
        Assert.Equal(2, versiones[1].Items.Count);
        Assert.All(versiones, v => Assert.NotNull(v.CreadoPor));

        var orden2 = (await admin.GetFromJsonAsync<OrdenReparacionDto>($"{Ruta}/{orden.Id}"))!;
        Assert.Equal(versiones[0].Id, orden2.PresupuestoVigente!.Id);
    }

    [Fact]
    public async Task Una_version_nueva_con_la_orden_aprobada_la_devuelve_a_esperando_aprobacion()
    {
        using var api = await TestApi.CrearAsync(postgres);
        var admin = api.ClienteAdministrador();
        var orden = await OrdenAsync(admin);
        await LlevarAsync(admin, orden.Id, HastaEsperandoPresupuesto);
        Assert.Equal(HttpStatusCode.OK, (await PresupuestarAsync(admin, orden.Id, ItemsValidos())).StatusCode);
        await LlevarAsync(admin, orden.Id, "esperando_aprobacion");
        await ResponderPresupuestoAsync(admin, orden.Id, aprobado: true);

        var respuesta = await PresupuestarAsync(admin, orden.Id, [new("Conector de carga", "repuesto", 8000m)]);

        Assert.Equal(HttpStatusCode.OK, respuesta.StatusCode);
        var actualizada = (await respuesta.Content.ReadFromJsonAsync<OrdenReparacionDto>())!;
        Assert.Equal("esperando_aprobacion", actualizada.Estado);
        Assert.Equal(2, actualizada.PresupuestoVigente!.Numero);
    }

    [Fact]
    public async Task Desde_presupuesto_rechazado_termina_en_presupuesto_enviado()
    {
        using var api = await TestApi.CrearAsync(postgres);
        var admin = api.ClienteAdministrador();
        var orden = await OrdenAsync(admin);
        await LlevarAsync(admin, orden.Id, HastaEsperandoPresupuesto);
        Assert.Equal(HttpStatusCode.OK, (await PresupuestarAsync(admin, orden.Id, ItemsValidos())).StatusCode);
        await LlevarAsync(admin, orden.Id, "esperando_aprobacion");
        await ResponderPresupuestoAsync(admin, orden.Id, aprobado: false);

        var respuesta = await PresupuestarAsync(admin, orden.Id, [new("Pantalla genérica", "repuesto", 9000m)]);

        Assert.Equal(HttpStatusCode.OK, respuesta.StatusCode);
        Assert.Equal("presupuesto_enviado", (await respuesta.Content.ReadFromJsonAsync<OrdenReparacionDto>())!.Estado);
    }

    [Fact]
    public async Task Rechaza_ordenes_inexistentes_entregadas_e_items_invalidos()
    {
        using var api = await TestApi.CrearAsync(postgres);
        var admin = api.ClienteAdministrador();
        var orden = await OrdenAsync(admin);
        await LlevarAsync(admin, orden.Id, HastaEsperandoPresupuesto);

        var inexistente = await PresupuestarAsync(admin, Guid.NewGuid(), ItemsValidos());
        Assert.Equal(HttpStatusCode.NotFound, inexistente.StatusCode);
        Assert.Equal("orden_inexistente", await CodigoAsync(inexistente));
        Assert.Equal(HttpStatusCode.NotFound, (await admin.GetAsync($"{Ruta}/{Guid.NewGuid()}/presupuestos")).StatusCode);

        var demasiados = Enumerable.Range(0, 31).Select(i => new ItemPresupuestoRequest($"Ítem {i}", "mano_obra", 10m)).ToList();
        foreach (var items in new List<ItemPresupuestoRequest>?[]
                 {
                     null,
                     [],
                     [new("Pantalla", "repuesto", -100m)],
                     [new("Pantalla", "regalo", 100m)],
                     [new("", "repuesto", 100m)],
                     demasiados,
                 })
        {
            var invalida = await PresupuestarAsync(admin, orden.Id, items);
            Assert.Equal(HttpStatusCode.BadRequest, invalida.StatusCode);
            Assert.Equal("presupuesto_invalido", await CodigoAsync(invalida));
        }

        var sinCambios = (await admin.GetFromJsonAsync<OrdenReparacionDto>($"{Ruta}/{orden.Id}"))!;
        Assert.Equal("esperando_presupuesto", sinCambios.Estado);
        Assert.Null(sinCambios.PresupuestoVigente);

        Assert.Equal(HttpStatusCode.OK, (await PresupuestarAsync(admin, orden.Id, ItemsValidos())).StatusCode);
        await LlevarAsync(admin, orden.Id, "esperando_aprobacion");
        await ResponderPresupuestoAsync(admin, orden.Id, aprobado: false);
        await EntregarAsync(admin, orden.Id);

        var entregada = await PresupuestarAsync(admin, orden.Id, ItemsValidos());
        Assert.Equal(HttpStatusCode.Conflict, entregada.StatusCode);
        Assert.Equal("orden_finalizada", await CodigoAsync(entregada));
        Assert.Single((await admin.GetFromJsonAsync<List<VersionDePresupuestoDto>>($"{Ruta}/{orden.Id}/presupuestos"))!);
    }

    [Fact]
    public async Task Matriz_de_autorizacion_para_presupuestar()
    {
        using var api = await TestApi.CrearAsync(postgres);
        var admin = api.ClienteAdministrador();
        var orden = await OrdenAsync(admin);
        await LlevarAsync(admin, orden.Id, "en_pruebas", "en_diagnostico");
        var recepcion = api.Cliente(await api.RegistrarUsuarioAsync("recepcion@x.com", RolesDeSistema.RecepcionVentasId));
        var tecnico = api.Cliente(await api.RegistrarUsuarioAsync("tecnico@x.com", RolesDeSistema.TecnicoId));
        var anonimo = api.Cliente();

        // Los tres roles de sistema tienen el permiso: para el 403 hace falta un rol que solo pueda ver reparaciones.
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

        Assert.Equal(HttpStatusCode.Forbidden, (await PresupuestarAsync(soloConsulta, orden.Id, ItemsValidos())).StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, (await PresupuestarAsync(anonimo, orden.Id, ItemsValidos())).StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, (await anonimo.GetAsync($"{Ruta}/{orden.Id}/presupuestos")).StatusCode);

        // En diagnóstico el estado no cambia, así que los tres roles pueden presupuestar seguido.
        Assert.Equal(HttpStatusCode.OK, (await PresupuestarAsync(recepcion, orden.Id, ItemsValidos())).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await PresupuestarAsync(tecnico, orden.Id, ItemsValidos())).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await PresupuestarAsync(admin, orden.Id, ItemsValidos())).StatusCode);

        // Quien solo ve reparaciones también ve el historial de presupuestos.
        var versiones = await soloConsulta.GetAsync($"{Ruta}/{orden.Id}/presupuestos");
        Assert.Equal(HttpStatusCode.OK, versiones.StatusCode);
        Assert.Equal([3, 2, 1], (await versiones.Content.ReadFromJsonAsync<List<VersionDePresupuestoDto>>())!.Select(v => v.Numero));
        Assert.Equal("en_diagnostico", (await admin.GetFromJsonAsync<OrdenReparacionDto>($"{Ruta}/{orden.Id}"))!.Estado);
    }
}
