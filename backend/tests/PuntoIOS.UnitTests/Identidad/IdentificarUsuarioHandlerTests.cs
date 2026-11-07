using PuntoIOS.Application.Common.Security;
using PuntoIOS.Application.Features.Identidad.IdentificarUsuario;
using PuntoIOS.Domain.Identidad;

namespace PuntoIOS.UnitTests.Identidad;

public class IdentificarUsuarioHandlerTests
{
    private static readonly DateTimeOffset Ahora = new(2026, 11, 7, 12, 0, 0, TimeSpan.Zero);

    private readonly UsuarioRepositorioEnMemoria _repo = new();
    private readonly RelojFijo _reloj = new(Ahora);

    private IdentificarUsuarioHandler Crear() => new(_repo, _reloj);

    private Usuario Registrado(string email, Rol? rol = null)
    {
        var usuario = Usuario.Registrar(email, "Nombre", rol ?? Roles.Administrador(), Ahora);
        _repo.Usuarios.Add(usuario);
        return usuario;
    }

    [Fact]
    public async Task Usuario_no_registrado_queda_sin_acceso()
    {
        var resultado = await Crear().HandleAsync(new CurrentUser("uid-x", "otro@x.com", true, null), default);

        Assert.Equal(EstadoIdentificacion.NoRegistrado, resultado.Estado);
        Assert.Null(resultado.Usuario);
    }

    [Fact]
    public async Task Primer_ingreso_con_email_verificado_vincula_el_uid_y_devuelve_rol_y_permisos()
    {
        var usuario = Registrado("dueno@puntoios.com");

        var resultado = await Crear().HandleAsync(new CurrentUser("uid-1", "Dueno@PuntoIOS.com", true, null), default);

        Assert.Equal(EstadoIdentificacion.Autorizado, resultado.Estado);
        Assert.Equal("uid-1", usuario.FirebaseUid);
        Assert.Equal(RolesDeSistema.Administrador, resultado.Usuario!.Rol);
        Assert.Contains(Permisos.UsuariosGestionar, resultado.Usuario.Permisos);
        Assert.Equal(1, _repo.Guardados);
    }

    [Fact]
    public async Task Email_sin_verificar_no_vincula_ni_da_acceso()
    {
        var usuario = Registrado("dueno@puntoios.com");

        var resultado = await Crear().HandleAsync(new CurrentUser("uid-1", "dueno@puntoios.com", false, null), default);

        Assert.Equal(EstadoIdentificacion.NoRegistrado, resultado.Estado);
        Assert.Null(usuario.FirebaseUid);
    }

    [Fact]
    public async Task Email_ya_vinculado_a_otro_uid_no_se_reasigna()
    {
        var usuario = Registrado("dueno@puntoios.com");
        usuario.VincularFirebase("uid-original", Ahora);

        var resultado = await Crear().HandleAsync(new CurrentUser("uid-intruso", "dueno@puntoios.com", true, null), default);

        Assert.Equal(EstadoIdentificacion.NoRegistrado, resultado.Estado);
        Assert.Equal("uid-original", usuario.FirebaseUid);
    }

    [Fact]
    public async Task Usuario_desactivado_no_obtiene_acceso_ni_al_vincularse()
    {
        var usuario = Registrado("dueno@puntoios.com");
        usuario.Desactivar();

        var resultado = await Crear().HandleAsync(new CurrentUser("uid-1", "dueno@puntoios.com", true, null), default);

        Assert.Equal(EstadoIdentificacion.Desactivado, resultado.Estado);
        Assert.Null(usuario.FirebaseUid);
    }

    [Fact]
    public async Task Usuario_vinculado_y_luego_desactivado_es_rechazado()
    {
        var usuario = Registrado("dueno@puntoios.com");
        usuario.VincularFirebase("uid-1", Ahora);
        usuario.Desactivar();

        var resultado = await Crear().HandleAsync(new CurrentUser("uid-1", "dueno@puntoios.com", true, null), default);

        Assert.Equal(EstadoIdentificacion.Desactivado, resultado.Estado);
    }

    [Fact]
    public async Task El_ultimo_ingreso_se_actualiza_como_mucho_cada_15_minutos()
    {
        var usuario = Registrado("dueno@puntoios.com");
        usuario.VincularFirebase("uid-1", Ahora);
        var identidad = new CurrentUser("uid-1", "dueno@puntoios.com", true, null);

        _reloj.Ahora = Ahora.AddMinutes(5);
        await Crear().HandleAsync(identidad, default);
        Assert.Equal(Ahora, usuario.UltimoIngresoEn);

        _reloj.Ahora = Ahora.AddMinutes(20);
        await Crear().HandleAsync(identidad, default);
        Assert.Equal(Ahora.AddMinutes(20), usuario.UltimoIngresoEn);
    }
}
