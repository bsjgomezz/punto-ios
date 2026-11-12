using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using PuntoIOS.Api.Controllers;
using PuntoIOS.Application.Common.Pagination;
using PuntoIOS.Application.Features.Auditoria;
using PuntoIOS.Application.Features.Clientes;
using PuntoIOS.Application.Features.Dispositivos;
using PuntoIOS.Application.Features.Reparaciones;
using PuntoIOS.Domain.Auditoria;
using PuntoIOS.Domain.Identidad;
using PuntoIOS.Domain.Reparaciones;

namespace PuntoIOS.IntegrationTests;

public class DatosTecnicosEndpointTests(PostgresFixture postgres) : IClassFixture<PostgresFixture>
{
    private const string Ruta = "/api/v1/reparaciones";

    private static async Task<OrdenReparacionDto> OrdenAsync(HttpClient admin)
    {
        var cliente = (await (await admin.PostAsJsonAsync("/api/v1/clientes", new CrearClienteRequest("Ana", "Pérez", "3498 111111", null, null, null)))
            .Content.ReadFromJsonAsync<ClienteDto>())!;
        var equipo = (await (await admin.PostAsJsonAsync(
                $"/api/v1/clientes/{cliente.Id}/dispositivos", new CrearDispositivoRequest("Apple", "iPhone 13", null, null, null, null)))
            .Content.ReadFromJsonAsync<DispositivoDto>())!;
        var creada = await admin.PostAsJsonAsync(
            Ruta, new RecibirEquipoRequest(cliente.Id, equipo.Id, null, "No carga", ["conector_carga"]));
        Assert.Equal(HttpStatusCode.Created, creada.StatusCode);
        return (await creada.Content.ReadFromJsonAsync<OrdenReparacionDto>())!;
    }

    private static Task<HttpResponseMessage> ActualizarAsync(HttpClient cliente, Guid ordenId, ActualizarDatosTecnicosRequest request) =>
        cliente.PutAsJsonAsync($"{Ruta}/{ordenId}/datos-tecnicos", request);

    private static async Task<string?> CodigoAsync(HttpResponseMessage respuesta) =>
        (await respuesta.Content.ReadFromJsonAsync<JsonElement>()).GetProperty("codigo").GetString();

    private static async Task<List<RegistroHistorialDto>> EdicionesAsync(HttpClient cliente, Guid ordenId) =>
        (await cliente.GetFromJsonAsync<PagedResult<RegistroHistorialDto>>($"{Ruta}/{ordenId}/historial"))!
            .Items.Where(h => h.Accion == AccionesAuditadas.Editado).ToList();

    private static readonly ActualizarDatosTecnicosRequest Datos = new(
        "Carga con otro cable", "Conector sulfatado", "0,2 A con 5 V", ["Conector de carga", "IC de carga"],
        "Cambio de conector de carga", "Cliente apurado");

    [Fact]
    public async Task El_tecnico_carga_los_datos_tecnicos_y_quedan_en_la_orden_y_en_el_historial()
    {
        using var api = await TestApi.CrearAsync(postgres);
        var admin = api.ClienteAdministrador();
        var orden = await OrdenAsync(admin);
        Assert.Empty(orden.ComponentesAfectados);
        Assert.Null(orden.TrabajoRealizado);
        Assert.Null(orden.NotasInternas);
        var tecnico = api.Cliente(await api.RegistrarUsuarioAsync("tecnico@x.com", RolesDeSistema.TecnicoId));

        var respuesta = await ActualizarAsync(tecnico, orden.Id, Datos);

        Assert.Equal(HttpStatusCode.OK, respuesta.StatusCode);
        var dto = (await respuesta.Content.ReadFromJsonAsync<OrdenReparacionDto>())!;
        Assert.Equal("Conector sulfatado", dto.Diagnostico);
        Assert.Equal(["Conector de carga", "IC de carga"], dto.ComponentesAfectados);
        Assert.Equal("Cambio de conector de carga", dto.TrabajoRealizado);

        var guardada = (await admin.GetFromJsonAsync<OrdenReparacionDto>($"{Ruta}/{orden.Id}"))!;
        Assert.Equal("Carga con otro cable", guardada.Pruebas);
        Assert.Equal("Conector sulfatado", guardada.Diagnostico);
        Assert.Equal("0,2 A con 5 V", guardada.Mediciones);
        Assert.Equal(["Conector de carga", "IC de carga"], guardada.ComponentesAfectados);
        Assert.Equal("Cambio de conector de carga", guardada.TrabajoRealizado);
        Assert.Equal("Cliente apurado", guardada.NotasInternas);

        var edicion = Assert.Single(await EdicionesAsync(admin, orden.Id));
        Assert.Equal(
            [
                new CambioDeCampo("pruebas", null, "Carga con otro cable"),
                new CambioDeCampo("diagnostico", null, "Conector sulfatado"),
                new CambioDeCampo("mediciones", null, "0,2 A con 5 V"),
                new CambioDeCampo("componentes_afectados", null, "Conector de carga, IC de carga"),
                new CambioDeCampo("trabajo_realizado", null, "Cambio de conector de carga"),
                new CambioDeCampo("notas_internas", null, "Cliente apurado"),
            ],
            edicion.Cambios);
        Assert.NotNull(edicion.UsuarioId);
    }

    [Fact]
    public async Task El_administrador_tambien_puede_y_solo_se_registra_lo_que_cambio()
    {
        using var api = await TestApi.CrearAsync(postgres);
        var admin = api.ClienteAdministrador();
        var orden = await OrdenAsync(admin);
        Assert.Equal(HttpStatusCode.OK, (await ActualizarAsync(admin, orden.Id, Datos)).StatusCode);

        var respuesta = await ActualizarAsync(admin, orden.Id, Datos with
        {
            Diagnostico = "Conector roto",
            ComponentesAfectados = ["Conector de carga"],
            TrabajoRealizado = "Cambio de conector y limpieza",
        });

        Assert.Equal(HttpStatusCode.OK, respuesta.StatusCode);
        var ediciones = await EdicionesAsync(admin, orden.Id);
        Assert.Equal(2, ediciones.Count);
        Assert.Equal(
            [
                new CambioDeCampo("diagnostico", "Conector sulfatado", "Conector roto"),
                new CambioDeCampo("componentes_afectados", "Conector de carga, IC de carga", "Conector de carga"),
                new CambioDeCampo("trabajo_realizado", "Cambio de conector de carga", "Cambio de conector y limpieza"),
            ],
            ediciones[0].Cambios);
    }

    [Fact]
    public async Task Repetir_los_mismos_valores_no_agrega_un_registro_al_historial()
    {
        using var api = await TestApi.CrearAsync(postgres);
        var admin = api.ClienteAdministrador();
        var orden = await OrdenAsync(admin);

        Assert.Equal(HttpStatusCode.OK, (await ActualizarAsync(admin, orden.Id, Datos)).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await ActualizarAsync(admin, orden.Id, Datos)).StatusCode);

        Assert.Single(await EdicionesAsync(admin, orden.Id));
    }

    [Fact]
    public async Task Rechaza_textos_demasiado_largos_demasiados_componentes_y_ordenes_inexistentes()
    {
        using var api = await TestApi.CrearAsync(postgres);
        var admin = api.ClienteAdministrador();
        var orden = await OrdenAsync(admin);
        var largo = new string('x', OrdenReparacion.LargoMaximoTextoTecnico + 1);
        var muchos = Enumerable.Range(1, OrdenReparacion.MaximoComponentesAfectados + 1).Select(i => $"Componente {i}").ToList();

        var textoLargo = await ActualizarAsync(admin, orden.Id, Datos with { NotasInternas = largo });
        var trabajoLargo = await ActualizarAsync(admin, orden.Id, Datos with { TrabajoRealizado = largo });
        var demasiados = await ActualizarAsync(admin, orden.Id, Datos with { ComponentesAfectados = muchos });
        var inexistente = await ActualizarAsync(admin, Guid.NewGuid(), Datos);

        Assert.Equal(HttpStatusCode.BadRequest, textoLargo.StatusCode);
        Assert.Equal("datos_tecnicos_invalidos", await CodigoAsync(textoLargo));
        Assert.Equal(HttpStatusCode.BadRequest, trabajoLargo.StatusCode);
        Assert.Equal("datos_tecnicos_invalidos", await CodigoAsync(trabajoLargo));
        Assert.Equal(HttpStatusCode.BadRequest, demasiados.StatusCode);
        Assert.Equal("datos_tecnicos_invalidos", await CodigoAsync(demasiados));
        Assert.Equal(HttpStatusCode.NotFound, inexistente.StatusCode);
        Assert.Equal("orden_inexistente", await CodigoAsync(inexistente));

        var sinCambios = (await admin.GetFromJsonAsync<OrdenReparacionDto>($"{Ruta}/{orden.Id}"))!;
        Assert.Null(sinCambios.Pruebas);
        Assert.Null(sinCambios.TrabajoRealizado);
        Assert.Empty(sinCambios.ComponentesAfectados);
        Assert.Empty(await EdicionesAsync(admin, orden.Id));
    }

    [Fact]
    public async Task Matriz_de_autorizacion_para_editar_los_datos_tecnicos()
    {
        using var api = await TestApi.CrearAsync(postgres);
        var admin = api.ClienteAdministrador();
        var orden = await OrdenAsync(admin);
        var recepcion = api.Cliente(await api.RegistrarUsuarioAsync("recepcion@x.com", RolesDeSistema.RecepcionVentasId));
        var tecnico = api.Cliente(await api.RegistrarUsuarioAsync("tecnico@x.com", RolesDeSistema.TecnicoId));
        var anonimo = api.Cliente();

        Assert.Equal(HttpStatusCode.OK, (await recepcion.GetAsync($"{Ruta}/{orden.Id}")).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await ActualizarAsync(recepcion, orden.Id, Datos)).StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, (await ActualizarAsync(anonimo, orden.Id, Datos)).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await ActualizarAsync(tecnico, orden.Id, Datos)).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await ActualizarAsync(admin, orden.Id, Datos with { Pruebas = "Otra" })).StatusCode);

        Assert.Equal(2, (await EdicionesAsync(admin, orden.Id)).Count);
    }
}
