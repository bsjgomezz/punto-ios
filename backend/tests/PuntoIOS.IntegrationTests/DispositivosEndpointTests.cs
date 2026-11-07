using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using PuntoIOS.Api.Controllers;
using PuntoIOS.Application.Common.Pagination;
using PuntoIOS.Application.Features.Auditoria;
using PuntoIOS.Application.Features.Clientes;
using PuntoIOS.Application.Features.Dispositivos;
using PuntoIOS.Domain.Identidad;

namespace PuntoIOS.IntegrationTests;

public class DispositivosEndpointTests(PostgresFixture postgres) : IClassFixture<PostgresFixture>
{
    private const string Imei = "490154203237518";
    private const string OtroImei = "356938035643809";

    private static CrearDispositivoRequest Equipo(string? imei = Imei, string modelo = "iPhone 13") =>
        new("Apple", modelo, "128 GB", "Azul", imei, null);

    private static async Task<ClienteDto> CrearClienteAsync(HttpClient http, string nombre = "Ana", string telefono = "3498 111111")
    {
        var respuesta = await http.PostAsJsonAsync("/api/v1/clientes", new CrearClienteRequest(nombre, "Pérez", telefono, null, null, null));
        return (await respuesta.Content.ReadFromJsonAsync<ClienteDto>())!;
    }

    [Fact]
    public async Task Alta_y_listado_devuelven_el_imei_enmascarado()
    {
        using var api = await TestApi.CrearAsync(postgres);
        var admin = api.ClienteAdministrador();
        var ana = await CrearClienteAsync(admin);

        var respuesta = await admin.PostAsJsonAsync($"/api/v1/clientes/{ana.Id}/dispositivos", Equipo());

        Assert.Equal(HttpStatusCode.Created, respuesta.StatusCode);
        var lista = await admin.GetFromJsonAsync<List<DispositivoDto>>($"/api/v1/clientes/{ana.Id}/dispositivos");
        var equipo = Assert.Single(lista!);
        Assert.Equal("•••••••••••7518", equipo.ImeiSerieEnmascarado);
        Assert.DoesNotContain(Imei, await admin.GetStringAsync($"/api/v1/clientes/{ana.Id}/dispositivos"));
    }

    [Fact]
    public async Task Un_imei_repetido_responde_409_con_el_cliente_que_lo_tiene()
    {
        using var api = await TestApi.CrearAsync(postgres);
        var admin = api.ClienteAdministrador();
        var ana = await CrearClienteAsync(admin);
        var luis = await CrearClienteAsync(admin, "Luis", "3498 222222");
        await admin.PostAsJsonAsync($"/api/v1/clientes/{ana.Id}/dispositivos", Equipo());

        var respuesta = await admin.PostAsJsonAsync($"/api/v1/clientes/{luis.Id}/dispositivos", Equipo(imei: "49 0154 2032 3751 8"));

        Assert.Equal(HttpStatusCode.Conflict, respuesta.StatusCode);
        var problema = await respuesta.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal("imei_duplicado", problema.GetProperty("codigo").GetString());
        Assert.Equal("Ana", problema.GetProperty("cliente").GetProperty("nombre").GetString());
    }

    [Fact]
    public async Task Validaciones_y_recursos_inexistentes()
    {
        using var api = await TestApi.CrearAsync(postgres);
        var admin = api.ClienteAdministrador();
        var ana = await CrearClienteAsync(admin);

        var imeiInvalido = await admin.PostAsJsonAsync($"/api/v1/clientes/{ana.Id}/dispositivos", Equipo(imei: "490154203237519"));
        var sinModelo = await admin.PostAsJsonAsync($"/api/v1/clientes/{ana.Id}/dispositivos", Equipo(modelo: ""));
        var clienteInexistente = await admin.PostAsJsonAsync($"/api/v1/clientes/{Guid.NewGuid()}/dispositivos", Equipo());

        Assert.Equal(HttpStatusCode.BadRequest, imeiInvalido.StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, sinModelo.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, clienteInexistente.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await admin.GetAsync($"/api/v1/dispositivos/{Guid.NewGuid()}")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await admin.PostAsync($"/api/v1/dispositivos/{Guid.NewGuid()}/imei", null)).StatusCode);
    }

    [Fact]
    public async Task Editar_sin_tocar_el_imei_lo_conserva_y_cambiarlo_queda_enmascarado_en_el_historial()
    {
        using var api = await TestApi.CrearAsync(postgres);
        var admin = api.ClienteAdministrador();
        var ana = await CrearClienteAsync(admin);
        var creado = await (await admin.PostAsJsonAsync($"/api/v1/clientes/{ana.Id}/dispositivos", Equipo()))
            .Content.ReadFromJsonAsync<DispositivoDto>();
        var ruta = $"/api/v1/dispositivos/{creado!.Id}";

        var sinImei = await admin.PutAsJsonAsync(ruta, new ActualizarDispositivoRequest("Apple", "iPhone 13 Pro", "128 GB", "Azul", null));
        Assert.Equal(HttpStatusCode.OK, sinImei.StatusCode);
        Assert.Equal("•••••••••••7518", (await admin.GetFromJsonAsync<DispositivoDto>(ruta))!.ImeiSerieEnmascarado);

        var conImei = await admin.PutAsJsonAsync(
            ruta, new ActualizarDispositivoRequest("Apple", "iPhone 13 Pro", "128 GB", "Azul", null, true, OtroImei));
        Assert.Equal(HttpStatusCode.OK, conImei.StatusCode);

        var historial = await admin.GetFromJsonAsync<PagedResult<RegistroHistorialDto>>($"{ruta}/historial");
        Assert.Equal(new[] { "editado", "editado", "creado" }, historial!.Items.Select(h => h.Accion));
        var cambioImei = Assert.Single(historial.Items[0].Cambios);
        Assert.Equal("•••••••••••3809", cambioImei.Nuevo);
        Assert.DoesNotContain(OtroImei, await admin.GetStringAsync($"{ruta}/historial"));
    }

    [Fact]
    public async Task Revelar_el_imei_lo_devuelve_completo_y_queda_en_el_historial()
    {
        using var api = await TestApi.CrearAsync(postgres);
        var admin = api.ClienteAdministrador();
        var ana = await CrearClienteAsync(admin);
        var creado = await (await admin.PostAsJsonAsync($"/api/v1/clientes/{ana.Id}/dispositivos", Equipo()))
            .Content.ReadFromJsonAsync<DispositivoDto>();

        var respuesta = await admin.PostAsync($"/api/v1/dispositivos/{creado!.Id}/imei", null);

        Assert.Equal(HttpStatusCode.OK, respuesta.StatusCode);
        Assert.Equal(Imei, (await respuesta.Content.ReadFromJsonAsync<ImeiDto>())!.ImeiSerie);
        var historial = await admin.GetFromJsonAsync<PagedResult<RegistroHistorialDto>>($"/api/v1/dispositivos/{creado.Id}/historial");
        Assert.Equal("imei_consultado", historial!.Items[0].Accion);
        Assert.False(string.IsNullOrWhiteSpace(historial.Items[0].Usuario));
    }

    [Fact]
    public async Task Matriz_de_autorizacion_de_equipos()
    {
        using var api = await TestApi.CrearAsync(postgres);
        var admin = api.ClienteAdministrador();
        var ana = await CrearClienteAsync(admin);
        var creado = await (await admin.PostAsJsonAsync($"/api/v1/clientes/{ana.Id}/dispositivos", Equipo()))
            .Content.ReadFromJsonAsync<DispositivoDto>();
        var recepcion = api.Cliente(await api.RegistrarUsuarioAsync("recepcion@x.com", RolesDeSistema.RecepcionVentasId));
        var tecnico = api.Cliente(await api.RegistrarUsuarioAsync("tecnico@x.com", RolesDeSistema.TecnicoId));
        var anonimo = api.Cliente();
        var lista = $"/api/v1/clientes/{ana.Id}/dispositivos";
        var equipo = $"/api/v1/dispositivos/{creado!.Id}";
        var edicion = new ActualizarDispositivoRequest("Apple", "iPhone 14", null, null, null);

        Assert.Equal(HttpStatusCode.Created, (await recepcion.PostAsJsonAsync(lista, Equipo(imei: OtroImei))).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await recepcion.PutAsJsonAsync(equipo, edicion)).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await recepcion.PostAsync($"{equipo}/imei", null)).StatusCode);

        Assert.Equal(HttpStatusCode.OK, (await tecnico.GetAsync(lista)).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await tecnico.PostAsync($"{equipo}/imei", null)).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await tecnico.PostAsJsonAsync(lista, Equipo(imei: null))).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await tecnico.PutAsJsonAsync(equipo, edicion)).StatusCode);

        Assert.Equal(HttpStatusCode.Unauthorized, (await anonimo.GetAsync(lista)).StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, (await anonimo.PostAsync($"{equipo}/imei", null)).StatusCode);
    }

    [Fact]
    public async Task Alta_de_cliente_con_equipo_guarda_los_dos_o_ninguno()
    {
        using var api = await TestApi.CrearAsync(postgres);
        var admin = api.ClienteAdministrador();
        var equipo = new EquipoNuevoRequest("Apple", "iPhone 13", "128 GB", "Azul", Imei);

        var conEquipo = await admin.PostAsJsonAsync(
            "/api/v1/clientes", new CrearClienteRequest("Ana", "Pérez", "3498 111111", null, null, null, false, equipo));
        Assert.Equal(HttpStatusCode.Created, conEquipo.StatusCode);
        var ana = await conEquipo.Content.ReadFromJsonAsync<ClienteDto>();
        var lista = await admin.GetFromJsonAsync<List<DispositivoDto>>($"/api/v1/clientes/{ana!.Id}/dispositivos");
        Assert.Equal("iPhone 13", Assert.Single(lista!).Modelo);

        var repetido = await admin.PostAsJsonAsync(
            "/api/v1/clientes", new CrearClienteRequest("Luis", "Gómez", "3498 222222", null, null, null, false, equipo));
        Assert.Equal(HttpStatusCode.Conflict, repetido.StatusCode);
        var clientes = await admin.GetFromJsonAsync<PagedResult<ClienteDto>>("/api/v1/clientes");
        Assert.Equal(1, clientes!.TotalCount);
    }
}
