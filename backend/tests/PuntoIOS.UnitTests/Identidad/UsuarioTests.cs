using PuntoIOS.Domain.Identidad;

namespace PuntoIOS.UnitTests.Identidad;

public class UsuarioTests
{
    private static readonly DateTimeOffset Ahora = new(2026, 11, 7, 12, 0, 0, TimeSpan.Zero);

    [Fact]
    public void Registrar_normaliza_el_email_y_queda_activo_sin_vincular()
    {
        var usuario = Usuario.Registrar("  Dueno@PuntoIOS.com ", " Dueño ", Roles.Administrador(), Ahora);

        Assert.Equal("dueno@puntoios.com", usuario.Email);
        Assert.Equal("Dueño", usuario.Nombre);
        Assert.True(usuario.Activo);
        Assert.Null(usuario.FirebaseUid);
        Assert.Equal(RolesDeSistema.AdministradorId, usuario.RolId);
    }

    [Theory]
    [InlineData("", "Nombre")]
    [InlineData("   ", "Nombre")]
    [InlineData("a@b.com", "")]
    public void Registrar_rechaza_datos_obligatorios_vacios(string email, string nombre)
    {
        Assert.Throws<ArgumentException>(() => Usuario.Registrar(email, nombre, Roles.Tecnico(), Ahora));
    }

    [Fact]
    public void Vincular_firebase_guarda_el_uid_y_no_permite_cambiarlo()
    {
        var usuario = Usuario.Registrar("a@b.com", "A", Roles.Tecnico(), Ahora);

        usuario.VincularFirebase("uid-1", Ahora);
        usuario.VincularFirebase("uid-1", Ahora); // mismo uid: idempotente

        Assert.Equal("uid-1", usuario.FirebaseUid);
        Assert.Throws<InvalidOperationException>(() => usuario.VincularFirebase("uid-2", Ahora));
    }

    [Fact]
    public void Cambiar_rol_actualiza_rol_e_identificador()
    {
        var usuario = Usuario.Registrar("a@b.com", "A", Roles.Tecnico(), Ahora);

        usuario.CambiarRol(Roles.Administrador());

        Assert.Equal(RolesDeSistema.AdministradorId, usuario.RolId);
        Assert.Equal(RolesDeSistema.Administrador, usuario.Rol.Nombre);
    }
}
