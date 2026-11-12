using System.Net;
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

public class CambiarEstadoEndpointTests(PostgresFixture postgres) : IClassFixture<PostgresFixture>
{
    private const string Ruta = "/api/v1/reparaciones";

    private static async Task<OrdenReparacionDto> OrdenAsync(HttpClient admin, string? desbloqueoTipo = null, string? desbloqueoValor = null)
    {
        var cliente = (await (await admin.PostAsJsonAsync("/api/v1/clientes", new CrearClienteRequest("Ana", "Pérez", "3498 111111", null, null, null)))
            .Content.ReadFromJsonAsync<ClienteDto>())!;
        var equipo = (await (await admin.PostAsJsonAsync(
                $"/api/v1/clientes/{cliente.Id}/dispositivos", new CrearDispositivoRequest("Apple", "iPhone 13", null, null, null, null)))
            .Content.ReadFromJsonAsync<DispositivoDto>())!;
        var creada = await admin.PostAsJsonAsync(
            Ruta,
            new RecibirEquipoRequest(cliente.Id, equipo.Id, null, "No carga", ["conector_carga"], null, null, null, desbloqueoTipo, desbloqueoValor));
        Assert.Equal(HttpStatusCode.Created, creada.StatusCode);
        return (await creada.Content.ReadFromJsonAsync<OrdenReparacionDto>())!;
    }

    private static Task<HttpResponseMessage> CambiarAsync(HttpClient cliente, Guid ordenId, string? estado) =>
        cliente.PostAsJsonAsync($"{Ruta}/{ordenId}/estado", new CambiarEstadoRequest(estado));

    private static async Task<string?> CodigoAsync(HttpResponseMessage respuesta) =>
        (await respuesta.Content.ReadFromJsonAsync<JsonElement>()).GetProperty("codigo").GetString();

    private static Task<HttpResponseMessage> PresupuestarAsync(HttpClient cliente, Guid ordenId) =>
        cliente.PostAsJsonAsync(
            $"{Ruta}/{ordenId}/presupuestos", new CrearVersionPresupuestoRequest([new("Conector de carga", "repuesto", 8000m)]));

    private static Task<HttpResponseMessage> ResponderPresupuestoAsync(HttpClient cliente, Guid ordenId, bool aprobado) =>
        cliente.PostAsJsonAsync($"{Ruta}/{ordenId}/presupuestos/respuesta", new ResponderPresupuestoRequest(aprobado));

    /// <summary>
    /// Lleva la orden por el flujo real hasta "esperando_aprobacion": cambio genérico hasta esperando presupuesto →
    /// se crea la versión 1 (queda en presupuesto enviado) → cambio genérico a esperando aprobación.
    /// </summary>
    private static async Task LlevarAEsperandoAprobacionAsync(HttpClient admin, Guid ordenId)
    {
        foreach (var estado in new[] { "en_pruebas", "en_diagnostico", "esperando_presupuesto" })
        {
            Assert.Equal(HttpStatusCode.OK, (await CambiarAsync(admin, ordenId, estado)).StatusCode);
        }

        var presupuesto = await PresupuestarAsync(admin, ordenId);
        Assert.Equal(HttpStatusCode.OK, presupuesto.StatusCode);
        Assert.Equal("presupuesto_enviado", (await presupuesto.Content.ReadFromJsonAsync<OrdenReparacionDto>())!.Estado);
        Assert.Equal(HttpStatusCode.OK, (await CambiarAsync(admin, ordenId, "esperando_aprobacion")).StatusCode);
    }

    [Fact]
    public async Task Recorre_una_cadena_valida_hasta_entregar_y_cada_paso_queda_en_el_historial()
    {
        using var api = await TestApi.CrearAsync(postgres);
        var admin = api.ClienteAdministrador();
        var orden = await OrdenAsync(admin);
        Assert.Equal(["en_pruebas", "cancelado"], orden.EstadosDisponibles);

        // Cada paso es un cambio de estado. Casi todos van por el cambio genérico; crear el presupuesto pasa a
        // "presupuesto_enviado" y rechazarlo (respuesta del presupuesto) pasa a "presupuesto_rechazado".
        (string Estado, Func<Task<HttpResponseMessage>> Paso)[] cadena =
        [
            ("en_pruebas", () => CambiarAsync(admin, orden.Id, "en_pruebas")),
            ("en_diagnostico", () => CambiarAsync(admin, orden.Id, "en_diagnostico")),
            ("esperando_presupuesto", () => CambiarAsync(admin, orden.Id, "esperando_presupuesto")),
            ("presupuesto_enviado", () => PresupuestarAsync(admin, orden.Id)),
            ("esperando_aprobacion", () => CambiarAsync(admin, orden.Id, "esperando_aprobacion")),
            ("presupuesto_rechazado", () => ResponderPresupuestoAsync(admin, orden.Id, aprobado: false)),
            ("entregado", () => CambiarAsync(admin, orden.Id, "entregado")),
        ];
        foreach (var (estado, paso) in cadena)
        {
            var respuesta = await paso();
            Assert.Equal(HttpStatusCode.OK, respuesta.StatusCode);
            Assert.Equal(estado, (await respuesta.Content.ReadFromJsonAsync<OrdenReparacionDto>())!.Estado);
        }

        var final = (await admin.GetFromJsonAsync<OrdenReparacionDto>($"{Ruta}/{orden.Id}"))!;
        Assert.Equal("entregado", final.Estado);
        Assert.Empty(final.EstadosDisponibles);

        var historial = await admin.GetFromJsonAsync<PagedResult<RegistroHistorialDto>>($"{Ruta}/{orden.Id}/historial");
        // Los siete pasos dejan su "estado_cambiado": crear y responder el presupuesto también registran el cambio de estado.
        Assert.Equal(cadena.Length, historial!.Items.Count(h => h.Accion == AccionesAuditadas.EstadoCambiado));
        Assert.Single(historial.Items, h => h.Accion == AccionesAuditadas.PresupuestoCreado);
        Assert.Single(historial.Items, h => h.Accion == AccionesAuditadas.PresupuestoRespondido);
        var ultimo = historial.Items[0];
        Assert.Equal(AccionesAuditadas.EstadoCambiado, ultimo.Accion);
        Assert.Equal(new CambioDeCampo("estado", "presupuesto_rechazado", "entregado"), Assert.Single(ultimo.Cambios));
    }

    [Fact]
    public async Task Rechaza_transiciones_invalidas_estados_inexistentes_y_ordenes_inexistentes()
    {
        using var api = await TestApi.CrearAsync(postgres);
        var admin = api.ClienteAdministrador();
        var orden = await OrdenAsync(admin);

        var directoAEntregado = await CambiarAsync(admin, orden.Id, "entregado");
        var mismoEstado = await CambiarAsync(admin, orden.Id, "recibido");
        var inexistente = await CambiarAsync(admin, orden.Id, "teletransportado");
        var vacio = await CambiarAsync(admin, orden.Id, null);
        var ordenInexistente = await CambiarAsync(admin, Guid.NewGuid(), "en_pruebas");

        Assert.Equal(HttpStatusCode.Conflict, directoAEntregado.StatusCode);
        Assert.Equal("transicion_invalida", await CodigoAsync(directoAEntregado));
        Assert.Equal(HttpStatusCode.Conflict, mismoEstado.StatusCode);
        Assert.Equal("transicion_invalida", await CodigoAsync(mismoEstado));
        Assert.Equal(HttpStatusCode.BadRequest, inexistente.StatusCode);
        Assert.Equal("estado_invalido", await CodigoAsync(inexistente));
        Assert.Equal(HttpStatusCode.BadRequest, vacio.StatusCode);
        Assert.Equal("estado_invalido", await CodigoAsync(vacio));
        Assert.Equal(HttpStatusCode.NotFound, ordenInexistente.StatusCode);
        Assert.Equal("orden_inexistente", await CodigoAsync(ordenInexistente));

        Assert.Equal("recibido", (await admin.GetFromJsonAsync<OrdenReparacionDto>($"{Ruta}/{orden.Id}"))!.Estado);
    }

    [Fact]
    public async Task Aprobar_o_rechazar_el_presupuesto_con_el_cambio_de_estado_generico_se_rechaza()
    {
        using var api = await TestApi.CrearAsync(postgres);
        var admin = api.ClienteAdministrador();
        var orden = await OrdenAsync(admin);
        await LlevarAEsperandoAprobacionAsync(admin, orden.Id);

        // Ni el detalle los ofrece: desde esperando aprobación no queda ningún estado para el cambio genérico.
        var enEspera = (await admin.GetFromJsonAsync<OrdenReparacionDto>($"{Ruta}/{orden.Id}"))!;
        Assert.Equal("esperando_aprobacion", enEspera.Estado);
        Assert.Empty(enEspera.EstadosDisponibles);

        foreach (var estado in new[] { "aprobado", "presupuesto_rechazado" })
        {
            var respuesta = await CambiarAsync(admin, orden.Id, estado);
            Assert.Equal(HttpStatusCode.Conflict, respuesta.StatusCode);
            Assert.Equal("usar_respuesta_presupuesto", await CodigoAsync(respuesta));
        }

        var sinCambios = (await admin.GetFromJsonAsync<OrdenReparacionDto>($"{Ruta}/{orden.Id}"))!;
        Assert.Equal("esperando_aprobacion", sinCambios.Estado);
        Assert.Null(sinCambios.PresupuestoVigente!.RespuestaAprobada);

        // Por la respuesta del presupuesto sí se aprueba.
        var aprobada = await ResponderPresupuestoAsync(admin, orden.Id, aprobado: true);
        Assert.Equal(HttpStatusCode.OK, aprobada.StatusCode);
        Assert.Equal("aprobado", (await aprobada.Content.ReadFromJsonAsync<OrdenReparacionDto>())!.Estado);
    }

    [Fact]
    public async Task Al_entregar_se_borra_el_desbloqueo_de_la_base()
    {
        using var api = await TestApi.CrearAsync(postgres);
        var admin = api.ClienteAdministrador();
        var orden = await OrdenAsync(admin, desbloqueoTipo: "pin", desbloqueoValor: "246810");
        Assert.Equal("pin", orden.DesbloqueoTipo);

        await LlevarAEsperandoAprobacionAsync(admin, orden.Id);
        Assert.Equal(HttpStatusCode.OK, (await ResponderPresupuestoAsync(admin, orden.Id, aprobado: false)).StatusCode);

        await using (var scope = api.Servicios.CreateAsyncScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<PuntoIosDbContext>();
            var antes = await db.OrdenesReparacion.AsNoTracking().SingleAsync(o => o.Id == orden.Id);
            Assert.NotNull(antes.DesbloqueoCifrado);
        }

        var entregada = await CambiarAsync(admin, orden.Id, "entregado");
        Assert.Equal(HttpStatusCode.OK, entregada.StatusCode);
        Assert.Null((await entregada.Content.ReadFromJsonAsync<OrdenReparacionDto>())!.DesbloqueoTipo);

        await using (var scope = api.Servicios.CreateAsyncScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<PuntoIosDbContext>();
            var guardada = await db.OrdenesReparacion.AsNoTracking().SingleAsync(o => o.Id == orden.Id);
            Assert.Null(guardada.DesbloqueoCifrado);
            Assert.Null(guardada.DesbloqueoTipo);
        }

        Assert.Equal(HttpStatusCode.NotFound, (await admin.PostAsync($"{Ruta}/{orden.Id}/desbloqueo", null)).StatusCode);
    }

    [Fact]
    public async Task Matriz_de_autorizacion_para_cambiar_el_estado()
    {
        using var api = await TestApi.CrearAsync(postgres);
        var admin = api.ClienteAdministrador();
        var orden = await OrdenAsync(admin);
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

        Assert.Equal(HttpStatusCode.OK, (await soloConsulta.GetAsync($"{Ruta}/{orden.Id}")).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await CambiarAsync(soloConsulta, orden.Id, "en_pruebas")).StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, (await CambiarAsync(anonimo, orden.Id, "en_pruebas")).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await CambiarAsync(recepcion, orden.Id, "en_pruebas")).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await CambiarAsync(tecnico, orden.Id, "en_diagnostico")).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await CambiarAsync(admin, orden.Id, "esperando_presupuesto")).StatusCode);
    }

    [Fact]
    public async Task Lista_los_quince_estados_con_su_nombre()
    {
        using var api = await TestApi.CrearAsync(postgres);
        var tecnico = api.Cliente(await api.RegistrarUsuarioAsync("tecnico@x.com", RolesDeSistema.TecnicoId));

        var estados = (await tecnico.GetFromJsonAsync<List<TipoReparacionDto>>($"{Ruta}/estados"))!;

        Assert.Equal(15, estados.Count);
        Assert.Equal(new TipoReparacionDto("recibido", "Recibido"), estados[0]);
        Assert.Contains(new TipoReparacionDto("en_diagnostico", "En diagnóstico"), estados);
        Assert.Contains(new TipoReparacionDto("irreparable", "Irreparable"), estados);
    }
}
