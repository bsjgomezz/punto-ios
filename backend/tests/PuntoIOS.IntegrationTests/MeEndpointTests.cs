using System.Net;
using System.Net.Http.Json;
using PuntoIOS.Application.Features.Identidad.ObtenerUsuarioActual;
using PuntoIOS.Domain.Identidad;

namespace PuntoIOS.IntegrationTests;

/// <summary>Verifica la validación del Firebase ID Token y la identificación del usuario local.</summary>
public class MeEndpointTests(PostgresFixture postgres) : IClassFixture<PostgresFixture>
{
    private const string Ruta = "/api/v1/me";

    [Fact]
    public async Task Sin_token_responde_401()
    {
        using var api = await TestApi.CrearAsync(postgres);

        var response = await api.Cliente().GetAsync(Ruta);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Con_un_token_basura_responde_401()
    {
        using var api = await TestApi.CrearAsync(postgres);

        var response = await api.Cliente("esto-no-es-un-jwt").GetAsync(Ruta);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Con_firma_de_otra_clave_responde_401()
    {
        using var api = await TestApi.CrearAsync(postgres);
        var token = TestApi.CrearToken("u", TestApi.EmailAdministrador, key: TestApi.OtraClave());

        var response = await api.Cliente(token).GetAsync(Ruta);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Con_otro_proyecto_de_firebase_responde_401()
    {
        using var api = await TestApi.CrearAsync(postgres);
        var token = TestApi.CrearToken("u", TestApi.EmailAdministrador, project: "otro-proyecto");

        var response = await api.Cliente(token).GetAsync(Ruta);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Con_token_vencido_responde_401()
    {
        using var api = await TestApi.CrearAsync(postgres);
        var token = TestApi.CrearToken("u", TestApi.EmailAdministrador, vigencia: TimeSpan.FromHours(-1));

        var response = await api.Cliente(token).GetAsync(Ruta);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Administrador_inicial_ingresa_y_queda_vinculado_a_su_uid()
    {
        using var api = await TestApi.CrearAsync(postgres);

        var response = await api.ClienteAdministrador().GetAsync(Ruta);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var yo = await response.Content.ReadFromJsonAsync<UsuarioActualDto>();
        Assert.Equal(TestApi.UidAdministrador, yo!.Uid);
        Assert.Equal(TestApi.EmailAdministrador, yo.Email);
        Assert.Equal(RolesDeSistema.Administrador, yo.Rol);
        Assert.NotNull(yo.UsuarioId);
        Assert.Equal(Permisos.Catalogo.Keys.Order(StringComparer.Ordinal), yo.Permisos);
    }

    [Fact]
    public async Task Usuario_de_firebase_no_registrado_responde_403()
    {
        using var api = await TestApi.CrearAsync(postgres);
        var token = TestApi.CrearToken("uid-ajeno", "ajeno@x.com");

        var response = await api.Cliente(token).GetAsync(Ruta);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Email_registrado_pero_sin_verificar_responde_403()
    {
        using var api = await TestApi.CrearAsync(postgres);
        var token = TestApi.CrearToken("uid-nuevo", TestApi.EmailAdministrador, emailVerificado: false);

        var response = await api.Cliente(token).GetAsync(Ruta);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Otro_uid_no_puede_apropiarse_del_email_del_administrador_ya_vinculado()
    {
        using var api = await TestApi.CrearAsync(postgres);
        await api.ClienteAdministrador().GetAsync(Ruta); // vincula uid-admin

        var intruso = TestApi.CrearToken("uid-intruso", TestApi.EmailAdministrador);
        var response = await api.Cliente(intruso).GetAsync(Ruta);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }
}
