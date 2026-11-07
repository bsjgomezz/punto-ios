using PuntoIOS.Application.Common.Results;
using PuntoIOS.Application.Features.Identidad.ActualizarUsuario;
using PuntoIOS.Application.Features.Identidad.RegistrarUsuario;
using PuntoIOS.Domain.Identidad;

namespace PuntoIOS.UnitTests.Identidad;

public class GestionDeUsuariosTests
{
    private static readonly DateTimeOffset Ahora = new(2026, 11, 7, 12, 0, 0, TimeSpan.Zero);

    private readonly Rol _admin = Roles.Administrador();
    private readonly Rol _tecnico = Roles.Tecnico();
    private readonly UsuarioRepositorioEnMemoria _repo = new();

    private RegistrarUsuarioHandler Registrar() => new(_repo, new RolRepositorioEnMemoria(_admin, _tecnico), new RelojFijo(Ahora));

    private ActualizarUsuarioHandler Actualizar() => new(_repo, new RolRepositorioEnMemoria(_admin, _tecnico));

    [Fact]
    public async Task Registrar_crea_el_usuario_con_email_normalizado()
    {
        var resultado = await Registrar().HandleAsync(new RegistrarUsuarioCommand(" Ana@Taller.com ", "Ana", _tecnico.Id), default);

        Assert.True(resultado.EsExitoso);
        Assert.Equal("ana@taller.com", resultado.Valor!.Email);
        Assert.Equal(RolesDeSistema.Tecnico, resultado.Valor.Rol);
        Assert.False(resultado.Valor.Vinculado);
        Assert.Single(_repo.Usuarios);
    }

    [Theory]
    [InlineData("no-es-un-email")]
    [InlineData("")]
    [InlineData("a b@c.com")]
    public async Task Registrar_rechaza_emails_invalidos(string email)
    {
        var resultado = await Registrar().HandleAsync(new RegistrarUsuarioCommand(email, "Ana", _tecnico.Id), default);

        Assert.Equal(TipoError.Validacion, resultado.Error!.Tipo);
        Assert.Empty(_repo.Usuarios);
    }

    [Fact]
    public async Task Registrar_rechaza_rol_inexistente_y_email_duplicado()
    {
        var rolInexistente = await Registrar().HandleAsync(new RegistrarUsuarioCommand("a@b.com", "A", Guid.NewGuid()), default);
        Assert.Equal("rol_inexistente", rolInexistente.Error!.Codigo);

        await Registrar().HandleAsync(new RegistrarUsuarioCommand("a@b.com", "A", _tecnico.Id), default);
        var duplicado = await Registrar().HandleAsync(new RegistrarUsuarioCommand("A@B.com", "Otra", _tecnico.Id), default);

        Assert.Equal(TipoError.Conflicto, duplicado.Error!.Tipo);
        Assert.Single(_repo.Usuarios);
    }

    [Fact]
    public async Task Actualizar_cambia_rol_y_estado()
    {
        var usuario = Usuario.Registrar("t@x.com", "T", _tecnico, Ahora);
        _repo.Usuarios.Add(usuario);

        var resultado = await Actualizar().HandleAsync(new ActualizarUsuarioCommand(usuario.Id, _admin.Id, false), default);

        Assert.True(resultado.EsExitoso);
        Assert.Equal(RolesDeSistema.AdministradorId, usuario.RolId);
        Assert.False(usuario.Activo);
    }

    [Fact]
    public async Task Actualizar_usuario_inexistente_responde_no_encontrado()
    {
        var resultado = await Actualizar().HandleAsync(new ActualizarUsuarioCommand(Guid.NewGuid(), null, false), default);

        Assert.Equal(TipoError.NoEncontrado, resultado.Error!.Tipo);
    }

    [Fact]
    public async Task No_se_puede_desactivar_ni_degradar_al_ultimo_administrador_activo()
    {
        var admin = Usuario.Registrar("admin@x.com", "Admin", _admin, Ahora);
        _repo.Usuarios.Add(admin);

        var desactivar = await Actualizar().HandleAsync(new ActualizarUsuarioCommand(admin.Id, null, false), default);
        var degradar = await Actualizar().HandleAsync(new ActualizarUsuarioCommand(admin.Id, _tecnico.Id, null), default);

        Assert.Equal("ultimo_administrador", desactivar.Error!.Codigo);
        Assert.Equal("ultimo_administrador", degradar.Error!.Codigo);
        Assert.True(admin.Activo);
        Assert.Equal(RolesDeSistema.AdministradorId, admin.RolId);
    }

    [Fact]
    public async Task Con_dos_administradores_se_puede_desactivar_uno()
    {
        var a = Usuario.Registrar("a@x.com", "A", _admin, Ahora);
        var b = Usuario.Registrar("b@x.com", "B", _admin, Ahora);
        _repo.Usuarios.AddRange([a, b]);

        var resultado = await Actualizar().HandleAsync(new ActualizarUsuarioCommand(a.Id, null, false), default);

        Assert.True(resultado.EsExitoso);
        Assert.False(a.Activo);
    }
}
