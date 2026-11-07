using System.Net;
using PuntoIOS.Domain.Identidad;

namespace PuntoIOS.IntegrationTests;

/// <summary>Matriz de autorización: quién puede llamar a cada endpoint protegido.</summary>
public class AutorizacionTests(PostgresFixture postgres) : IClassFixture<PostgresFixture>
{
    public static TheoryData<string> Endpoints => new() { "/api/v1/usuarios", "/api/v1/roles" };

    [Theory]
    [MemberData(nameof(Endpoints))]
    public async Task Sin_token_responde_401(string ruta)
    {
        using var api = await TestApi.CrearAsync(postgres);

        Assert.Equal(HttpStatusCode.Unauthorized, (await api.Cliente().GetAsync(ruta)).StatusCode);
    }

    [Theory]
    [MemberData(nameof(Endpoints))]
    public async Task Administrador_puede_consultar(string ruta)
    {
        using var api = await TestApi.CrearAsync(postgres);

        Assert.Equal(HttpStatusCode.OK, (await api.ClienteAdministrador().GetAsync(ruta)).StatusCode);
    }

    [Theory]
    [MemberData(nameof(Endpoints))]
    public async Task Recepcion_y_tecnico_no_tienen_permiso_y_reciben_403(string ruta)
    {
        using var api = await TestApi.CrearAsync(postgres);
        var recepcion = await api.RegistrarUsuarioAsync("recepcion@x.com", RolesDeSistema.RecepcionVentasId);
        var tecnico = await api.RegistrarUsuarioAsync("tecnico@x.com", RolesDeSistema.TecnicoId);

        Assert.Equal(HttpStatusCode.Forbidden, (await api.Cliente(recepcion).GetAsync(ruta)).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await api.Cliente(tecnico).GetAsync(ruta)).StatusCode);
    }

    [Theory]
    [MemberData(nameof(Endpoints))]
    public async Task Usuario_desactivado_recibe_403_aunque_su_rol_tenga_permiso(string ruta)
    {
        using var api = await TestApi.CrearAsync(postgres);
        var desactivado = await api.RegistrarUsuarioAsync("baja@x.com", RolesDeSistema.AdministradorId, activo: false);

        Assert.Equal(HttpStatusCode.Forbidden, (await api.Cliente(desactivado).GetAsync(ruta)).StatusCode);
    }

    [Theory]
    [MemberData(nameof(Endpoints))]
    public async Task Usuario_no_registrado_recibe_403(string ruta)
    {
        using var api = await TestApi.CrearAsync(postgres);
        var token = TestApi.CrearToken("uid-ajeno", "ajeno@x.com");

        Assert.Equal(HttpStatusCode.Forbidden, (await api.Cliente(token).GetAsync(ruta)).StatusCode);
    }
}
