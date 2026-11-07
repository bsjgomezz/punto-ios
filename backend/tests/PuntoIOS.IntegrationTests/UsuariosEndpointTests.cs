using System.Net;
using System.Net.Http.Json;
using PuntoIOS.Api.Controllers;
using PuntoIOS.Application.Common.Pagination;
using PuntoIOS.Application.Features.Identidad;
using PuntoIOS.Domain.Identidad;

namespace PuntoIOS.IntegrationTests;

public class UsuariosEndpointTests(PostgresFixture postgres) : IClassFixture<PostgresFixture>
{
    [Fact]
    public async Task Los_roles_de_sistema_vienen_sembrados_con_sus_permisos()
    {
        using var api = await TestApi.CrearAsync(postgres);

        var roles = await api.ClienteAdministrador().GetFromJsonAsync<List<RolDto>>("/api/v1/roles");

        Assert.NotNull(roles);
        string[] esperados = [RolesDeSistema.Administrador, RolesDeSistema.RecepcionVentas, RolesDeSistema.Tecnico];
        Assert.Equal(esperados.Order(StringComparer.Ordinal), roles.Select(r => r.Nombre).Order(StringComparer.Ordinal));
        Assert.Equal(Permisos.Catalogo.Count, roles.Single(r => r.Id == RolesDeSistema.AdministradorId).Permisos.Count);
        Assert.Equal(
            [Permisos.ClientesVer, Permisos.DispositivosVer, Permisos.DispositivosVerImei],
            roles.Single(r => r.Id == RolesDeSistema.TecnicoId).Permisos);
        Assert.Equal(
            [
                Permisos.ClientesCrear, Permisos.ClientesEditar, Permisos.ClientesSeguimiento, Permisos.ClientesVer,
                Permisos.DispositivosCrear, Permisos.DispositivosEditar, Permisos.DispositivosVer, Permisos.DispositivosVerImei,
            ],
            roles.Single(r => r.Id == RolesDeSistema.RecepcionVentasId).Permisos);
    }

    [Fact]
    public async Task Listar_devuelve_la_respuesta_paginada()
    {
        using var api = await TestApi.CrearAsync(postgres);
        await api.RegistrarUsuarioAsync("a@x.com", RolesDeSistema.TecnicoId);
        await api.RegistrarUsuarioAsync("b@x.com", RolesDeSistema.TecnicoId);

        var pagina = await api.ClienteAdministrador()
            .GetFromJsonAsync<PagedResult<UsuarioDto>>("/api/v1/usuarios?page=1&pageSize=2");

        Assert.Equal(2, pagina!.Items.Count);
        Assert.Equal(3, pagina.TotalCount); // administrador inicial + 2
        Assert.Equal(2, pagina.TotalPages);
    }

    [Fact]
    public async Task Registrar_crea_el_usuario_y_luego_puede_ingresar_con_su_email_verificado()
    {
        using var api = await TestApi.CrearAsync(postgres);
        var admin = api.ClienteAdministrador();

        var creado = await admin.PostAsJsonAsync("/api/v1/usuarios",
            new RegistrarUsuarioRequest("Nuevo@Taller.com", "Nuevo", RolesDeSistema.TecnicoId));

        Assert.Equal(HttpStatusCode.Created, creado.StatusCode);
        var token = TestApi.CrearToken("uid-nuevo", "nuevo@taller.com");
        Assert.Equal(HttpStatusCode.OK, (await api.Cliente(token).GetAsync("/api/v1/me")).StatusCode);
    }

    [Fact]
    public async Task Registrar_con_email_repetido_responde_409_y_con_email_invalido_400()
    {
        using var api = await TestApi.CrearAsync(postgres);
        var admin = api.ClienteAdministrador();

        var repetido = await admin.PostAsJsonAsync("/api/v1/usuarios",
            new RegistrarUsuarioRequest(TestApi.EmailAdministrador, "Otro", RolesDeSistema.TecnicoId));
        var invalido = await admin.PostAsJsonAsync("/api/v1/usuarios",
            new RegistrarUsuarioRequest("no-es-email", "Otro", RolesDeSistema.TecnicoId));

        Assert.Equal(HttpStatusCode.Conflict, repetido.StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, invalido.StatusCode);
    }

    [Fact]
    public async Task Quien_no_gestiona_usuarios_no_puede_registrar()
    {
        using var api = await TestApi.CrearAsync(postgres);
        var tecnico = await api.RegistrarUsuarioAsync("tec@x.com", RolesDeSistema.TecnicoId);

        var response = await api.Cliente(tecnico).PostAsJsonAsync("/api/v1/usuarios",
            new RegistrarUsuarioRequest("otro@x.com", "Otro", RolesDeSistema.TecnicoId));

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Desactivar_a_un_usuario_le_quita_el_acceso_y_no_se_puede_dejar_sin_administradores()
    {
        using var api = await TestApi.CrearAsync(postgres);
        var admin = api.ClienteAdministrador();
        var tokenTecnico = await api.RegistrarUsuarioAsync("tec@x.com", RolesDeSistema.TecnicoId);
        var pagina = await admin.GetFromJsonAsync<PagedResult<UsuarioDto>>("/api/v1/usuarios");
        var tecnico = pagina!.Items.Single(u => u.Email == "tec@x.com");
        var yo = pagina.Items.Single(u => u.Email == TestApi.EmailAdministrador);

        Assert.Equal(HttpStatusCode.OK, (await api.Cliente(tokenTecnico).GetAsync("/api/v1/me")).StatusCode);

        var baja = await admin.PatchAsJsonAsync($"/api/v1/usuarios/{tecnico.Id}", new ActualizarUsuarioRequest(null, false));
        Assert.Equal(HttpStatusCode.OK, baja.StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await api.Cliente(tokenTecnico).GetAsync("/api/v1/me")).StatusCode);

        var ultimo = await admin.PatchAsJsonAsync($"/api/v1/usuarios/{yo.Id}", new ActualizarUsuarioRequest(null, false));
        Assert.Equal(HttpStatusCode.Conflict, ultimo.StatusCode);

        var inexistente = await admin.PatchAsJsonAsync($"/api/v1/usuarios/{Guid.NewGuid()}", new ActualizarUsuarioRequest(null, false));
        Assert.Equal(HttpStatusCode.NotFound, inexistente.StatusCode);
    }
}
