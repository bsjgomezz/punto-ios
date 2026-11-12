using System.Net;
using System.Threading;
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

public class ResponderPresupuestoEndpointTests(PostgresFixture postgres) : IClassFixture<PostgresFixture>
{
    private const string Ruta = "/api/v1/reparaciones";

    private static readonly string[] HastaEsperandoPresupuesto = ["en_pruebas", "en_diagnostico", "esperando_presupuesto"];

    private static int _contadorDeClientes;

    // Cada llamada crea un cliente con un teléfono distinto: este helper se usa varias veces dentro del mismo test
    // (p. ej. la matriz de autorización crea 4 órdenes), y un teléfono repetido en el mismo local haría que
    // CrearClienteHandler devuelva "teléfono duplicado" en lugar del cliente, rompiendo todo lo que sigue.
    private static async Task<OrdenReparacionDto> OrdenAsync(HttpClient admin)
    {
        var telefono = $"3498 1111{Interlocked.Increment(ref _contadorDeClientes):D2}";
        var cliente = (await (await admin.PostAsJsonAsync("/api/v1/clientes", new CrearClienteRequest("Ana", "Pérez", telefono, null, null, null)))
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

    /// <summary>
    /// Orden nueva llevada por el flujo real hasta "esperando_aprobacion": esperando presupuesto → se crea la versión 1
    /// (queda en presupuesto enviado) → esperando aprobación.
    /// </summary>
    private static async Task<OrdenReparacionDto> OrdenEsperandoAprobacionAsync(HttpClient admin)
    {
        var orden = await OrdenAsync(admin);
        await LlevarAsync(admin, orden.Id, HastaEsperandoPresupuesto);
        var presupuesto = await admin.PostAsJsonAsync(
            $"{Ruta}/{orden.Id}/presupuestos",
            new CrearVersionPresupuestoRequest([new("Pantalla OLED", "repuesto", 15000m), new("Cambio de pantalla", "mano_obra", 2500.50m)]));
        Assert.Equal(HttpStatusCode.OK, presupuesto.StatusCode);
        Assert.Equal("presupuesto_enviado", (await presupuesto.Content.ReadFromJsonAsync<OrdenReparacionDto>())!.Estado);
        await LlevarAsync(admin, orden.Id, "esperando_aprobacion");
        return orden;
    }

    private static Task<HttpResponseMessage> ResponderAsync(HttpClient cliente, Guid ordenId, bool aprobado) =>
        cliente.PostAsJsonAsync($"{Ruta}/{ordenId}/presupuestos/respuesta", new ResponderPresupuestoRequest(aprobado));

    private static async Task<string?> CodigoAsync(HttpResponseMessage respuesta) =>
        (await respuesta.Content.ReadFromJsonAsync<JsonElement>()).GetProperty("codigo").GetString();

    [Fact]
    public async Task Aprobar_pasa_la_orden_a_aprobado_y_guarda_la_respuesta_en_la_version_vigente()
    {
        using var api = await TestApi.CrearAsync(postgres);
        var admin = api.ClienteAdministrador();
        var orden = await OrdenEsperandoAprobacionAsync(admin);

        var respuesta = await ResponderAsync(admin, orden.Id, true);

        Assert.Equal(HttpStatusCode.OK, respuesta.StatusCode);
        var actualizada = (await respuesta.Content.ReadFromJsonAsync<OrdenReparacionDto>())!;
        Assert.Equal("aprobado", actualizada.Estado);
        Assert.True(actualizada.PresupuestoVigente!.RespuestaAprobada);
        Assert.NotNull(actualizada.PresupuestoVigente.RespondidoEn);
        Assert.NotNull(actualizada.PresupuestoVigente.RespondidoPor);

        var leida = (await admin.GetFromJsonAsync<OrdenReparacionDto>($"{Ruta}/{orden.Id}"))!;
        Assert.Equal("aprobado", leida.Estado);
        Assert.True(leida.PresupuestoVigente!.RespuestaAprobada);

        var historial = (await admin.GetFromJsonAsync<PagedResult<RegistroHistorialDto>>($"{Ruta}/{orden.Id}/historial"))!;
        var respondido = Assert.Single(historial.Items, h => h.Accion == AccionesAuditadas.PresupuestoRespondido);
        Assert.Equal(
            [new CambioDeCampo("version_presupuesto", null, "1"), new CambioDeCampo("respuesta", null, "aprobado")],
            respondido.Cambios);
        Assert.Contains(
            historial.Items,
            h => h.Accion == AccionesAuditadas.EstadoCambiado
                && h.Cambios.SequenceEqual(new[] { new CambioDeCampo("estado", "esperando_aprobacion", "aprobado") }));
    }

    [Fact]
    public async Task Rechazar_pasa_la_orden_a_presupuesto_rechazado()
    {
        using var api = await TestApi.CrearAsync(postgres);
        var admin = api.ClienteAdministrador();
        var orden = await OrdenEsperandoAprobacionAsync(admin);

        var respuesta = await ResponderAsync(admin, orden.Id, false);

        Assert.Equal(HttpStatusCode.OK, respuesta.StatusCode);
        var actualizada = (await respuesta.Content.ReadFromJsonAsync<OrdenReparacionDto>())!;
        Assert.Equal("presupuesto_rechazado", actualizada.Estado);
        Assert.False(actualizada.PresupuestoVigente!.RespuestaAprobada);
    }

    [Fact]
    public async Task Responder_dos_veces_la_misma_version_da_conflicto_y_conserva_la_primera_respuesta()
    {
        using var api = await TestApi.CrearAsync(postgres);
        var admin = api.ClienteAdministrador();
        var orden = await OrdenEsperandoAprobacionAsync(admin);
        Assert.Equal(HttpStatusCode.OK, (await ResponderAsync(admin, orden.Id, true)).StatusCode);

        // Ya no está esperando aprobación: el primer chequeo que falla es el de estado.
        var segunda = await ResponderAsync(admin, orden.Id, false);
        Assert.Equal(HttpStatusCode.Conflict, segunda.StatusCode);
        Assert.Equal("fuera_de_esperando_aprobacion", await CodigoAsync(segunda));

        // Aunque la orden vuelva a "esperando_aprobacion" sin versión nueva, la misma versión no se responde otra vez.
        await LlevarAsync(admin, orden.Id, "esperando_aprobacion");
        var tercera = await ResponderAsync(admin, orden.Id, false);
        Assert.Equal(HttpStatusCode.Conflict, tercera.StatusCode);
        Assert.Equal("presupuesto_ya_respondido", await CodigoAsync(tercera));

        var version = Assert.Single((await admin.GetFromJsonAsync<List<VersionDePresupuestoDto>>($"{Ruta}/{orden.Id}/presupuestos"))!);
        Assert.True(version.RespuestaAprobada);
        Assert.Equal("esperando_aprobacion", (await admin.GetFromJsonAsync<OrdenReparacionDto>($"{Ruta}/{orden.Id}"))!.Estado);
    }

    [Fact]
    public async Task Una_orden_que_no_esta_esperando_aprobacion_o_inexistente_no_se_puede_responder()
    {
        using var api = await TestApi.CrearAsync(postgres);
        var admin = api.ClienteAdministrador();
        var recibida = await OrdenAsync(admin);

        var fueraDeTurno = await ResponderAsync(admin, recibida.Id, true);
        Assert.Equal(HttpStatusCode.Conflict, fueraDeTurno.StatusCode);
        Assert.Equal("fuera_de_esperando_aprobacion", await CodigoAsync(fueraDeTurno));
        Assert.Equal("recibido", (await admin.GetFromJsonAsync<OrdenReparacionDto>($"{Ruta}/{recibida.Id}"))!.Estado);

        var inexistente = await ResponderAsync(admin, Guid.NewGuid(), true);
        Assert.Equal(HttpStatusCode.NotFound, inexistente.StatusCode);
        Assert.Equal("orden_inexistente", await CodigoAsync(inexistente));
    }

    [Fact]
    public async Task Listar_versiones_despues_de_responder_trae_la_respuesta_solo_en_la_version_respondida()
    {
        using var api = await TestApi.CrearAsync(postgres);
        var admin = api.ClienteAdministrador();
        var orden = await OrdenEsperandoAprobacionAsync(admin);
        Assert.Equal(HttpStatusCode.OK, (await ResponderAsync(admin, orden.Id, false)).StatusCode);

        // Una versión nueva desde rechazado vuelve a presupuesto enviado; se la responde aprobando.
        var nueva = await admin.PostAsJsonAsync(
            $"{Ruta}/{orden.Id}/presupuestos", new CrearVersionPresupuestoRequest([new("Pantalla genérica", "repuesto", 9000m)]));
        Assert.Equal(HttpStatusCode.OK, nueva.StatusCode);
        var sinResponder = (await nueva.Content.ReadFromJsonAsync<OrdenReparacionDto>())!.PresupuestoVigente!;
        Assert.Null(sinResponder.RespuestaAprobada);
        Assert.Null(sinResponder.RespondidoEn);
        Assert.Null(sinResponder.RespondidoPor);

        await LlevarAsync(admin, orden.Id, "esperando_aprobacion");
        Assert.Equal(HttpStatusCode.OK, (await ResponderAsync(admin, orden.Id, true)).StatusCode);

        var versiones = (await admin.GetFromJsonAsync<List<VersionDePresupuestoDto>>($"{Ruta}/{orden.Id}/presupuestos"))!;
        Assert.Equal([2, 1], versiones.Select(v => v.Numero));
        Assert.True(versiones[0].RespuestaAprobada);
        Assert.False(versiones[1].RespuestaAprobada);
        Assert.All(versiones, v =>
        {
            Assert.NotNull(v.RespondidoEn);
            Assert.NotNull(v.RespondidoPor);
        });
    }

    [Fact]
    public async Task Matriz_de_autorizacion_para_responder_el_presupuesto()
    {
        using var api = await TestApi.CrearAsync(postgres);
        var admin = api.ClienteAdministrador();
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

        var orden = await OrdenEsperandoAprobacionAsync(admin);
        Assert.Equal(HttpStatusCode.Forbidden, (await ResponderAsync(soloConsulta, orden.Id, true)).StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, (await ResponderAsync(anonimo, orden.Id, true)).StatusCode);
        Assert.Equal("esperando_aprobacion", (await admin.GetFromJsonAsync<OrdenReparacionDto>($"{Ruta}/{orden.Id}"))!.Estado);

        foreach (var cliente in new[] { recepcion, tecnico, admin })
        {
            var otra = await OrdenEsperandoAprobacionAsync(admin);
            var respuesta = await ResponderAsync(cliente, otra.Id, true);
            Assert.Equal(HttpStatusCode.OK, respuesta.StatusCode);
            Assert.Equal("aprobado", (await respuesta.Content.ReadFromJsonAsync<OrdenReparacionDto>())!.Estado);
        }
    }
}
