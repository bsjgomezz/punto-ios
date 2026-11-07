using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using PuntoIOS.Api.Controllers;
using PuntoIOS.Application.Common.Pagination;
using PuntoIOS.Application.Features.Clientes;
using PuntoIOS.Domain.Identidad;

namespace PuntoIOS.IntegrationTests;

public class ClientesEndpointTests(PostgresFixture postgres) : IClassFixture<PostgresFixture>
{
    private const string Ruta = "/api/v1/clientes";

    private static CrearClienteRequest Cliente(
        string nombre = "Ana", string apellido = "Pérez", string telefono = "3498 123456", bool permitirDuplicado = false) =>
        new(nombre, apellido, telefono, null, null, null, permitirDuplicado);

    [Fact]
    public async Task Crear_responde_201_con_el_cliente_y_se_puede_consultar()
    {
        using var api = await TestApi.CrearAsync(postgres);
        var admin = api.ClienteAdministrador();

        var respuesta = await admin.PostAsJsonAsync(Ruta, new CrearClienteRequest("Ana", "Pérez", "3498 123456", "Ana@Mail.com", "Calle 1", "VIP"));

        Assert.Equal(HttpStatusCode.Created, respuesta.StatusCode);
        var creado = await respuesta.Content.ReadFromJsonAsync<ClienteDto>();
        Assert.Equal("ana@mail.com", creado!.Email);
        Assert.NotNull(respuesta.Headers.Location);

        var consultado = await admin.GetFromJsonAsync<ClienteDto>($"{Ruta}/{creado.Id}");
        Assert.Equal("Pérez", consultado!.Apellido);
        Assert.Equal(HttpStatusCode.NotFound, (await admin.GetAsync($"{Ruta}/{Guid.NewGuid()}")).StatusCode);
    }

    [Fact]
    public async Task Crear_con_datos_invalidos_responde_400_con_codigo()
    {
        using var api = await TestApi.CrearAsync(postgres);

        var respuesta = await api.ClienteAdministrador().PostAsJsonAsync(Ruta, Cliente(telefono: "123"));

        Assert.Equal(HttpStatusCode.BadRequest, respuesta.StatusCode);
        var problema = await respuesta.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal("telefono_invalido", problema.GetProperty("codigo").GetString());
    }

    [Fact]
    public async Task Telefono_repetido_responde_409_con_coincidencias_y_se_puede_confirmar()
    {
        using var api = await TestApi.CrearAsync(postgres);
        var admin = api.ClienteAdministrador();
        await admin.PostAsJsonAsync(Ruta, Cliente(telefono: "3498123456"));

        var repetido = await admin.PostAsJsonAsync(Ruta, Cliente(nombre: "Luis", telefono: "+54 9 3498 12-3456"));

        Assert.Equal(HttpStatusCode.Conflict, repetido.StatusCode);
        var problema = await repetido.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal("telefono_duplicado", problema.GetProperty("codigo").GetString());
        Assert.Equal("Ana", problema.GetProperty("coincidencias")[0].GetProperty("nombre").GetString());

        var confirmado = await admin.PostAsJsonAsync(Ruta, Cliente(nombre: "Luis", telefono: "3498123456", permitirDuplicado: true));
        Assert.Equal(HttpStatusCode.Created, confirmado.StatusCode);
    }

    [Fact]
    public async Task Buscar_por_nombre_apellido_y_telefono_con_otro_formato()
    {
        using var api = await TestApi.CrearAsync(postgres);
        var admin = api.ClienteAdministrador();
        await admin.PostAsJsonAsync(Ruta, Cliente("Ana", "Pérez", "3498 111111"));
        await admin.PostAsJsonAsync(Ruta, Cliente("Luis", "Gómez", "3498 222222"));
        await admin.PostAsJsonAsync(Ruta, Cliente("Ana", "Gómez", "3498 333333"));

        async Task<List<string>> Buscar(string q) =>
            (await admin.GetFromJsonAsync<PagedResult<ClienteDto>>($"{Ruta}?q={Uri.EscapeDataString(q)}"))!
                .Items.Select(c => $"{c.Nombre} {c.Apellido}").ToList();

        Assert.Equal(["Ana Gómez", "Luis Gómez", "Ana Pérez"], await Buscar(""));
        Assert.Equal(["Ana Gómez", "Ana Pérez"], await Buscar("ana"));
        Assert.Equal(["Ana Gómez"], await Buscar("ana gómez"));
        Assert.Equal(["Luis Gómez"], await Buscar("+54 9 3498-22-2222"));
        Assert.Equal(["Ana Pérez"], await Buscar("111"));
        Assert.Empty(await Buscar("zzz"));
        Assert.Empty(await Buscar("%"));
    }

    [Fact]
    public async Task Buscar_es_paginado()
    {
        using var api = await TestApi.CrearAsync(postgres);
        var admin = api.ClienteAdministrador();
        for (var i = 0; i < 5; i++)
        {
            await admin.PostAsJsonAsync(Ruta, Cliente("Cliente", $"Apellido{i}", $"349811000{i}"));
        }

        var pagina = await admin.GetFromJsonAsync<PagedResult<ClienteDto>>($"{Ruta}?page=2&pageSize=2");

        Assert.Equal(2, pagina!.Items.Count);
        Assert.Equal(5, pagina.TotalCount);
        Assert.Equal(3, pagina.TotalPages);
        Assert.Equal("Apellido2", pagina.Items[0].Apellido);
    }

    [Fact]
    public async Task Matriz_de_autorizacion_de_clientes()
    {
        using var api = await TestApi.CrearAsync(postgres);
        var recepcion = api.Cliente(await api.RegistrarUsuarioAsync("recepcion@x.com", RolesDeSistema.RecepcionVentasId));
        var tecnico = api.Cliente(await api.RegistrarUsuarioAsync("tecnico@x.com", RolesDeSistema.TecnicoId));

        // Recepción: consulta y crea.
        Assert.Equal(HttpStatusCode.OK, (await recepcion.GetAsync(Ruta)).StatusCode);
        Assert.Equal(HttpStatusCode.Created, (await recepcion.PostAsJsonAsync(Ruta, Cliente())).StatusCode);

        // Técnico: solo consulta.
        Assert.Equal(HttpStatusCode.OK, (await tecnico.GetAsync(Ruta)).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await tecnico.PostAsJsonAsync(Ruta, Cliente(telefono: "3498999999"))).StatusCode);

        // Sin sesión.
        var anonimo = api.Cliente();
        Assert.Equal(HttpStatusCode.Unauthorized, (await anonimo.GetAsync(Ruta)).StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, (await anonimo.PostAsJsonAsync(Ruta, Cliente())).StatusCode);
    }
}
