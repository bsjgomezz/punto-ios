using PuntoIOS.Application.Common.Security;
using PuntoIOS.Application.Features.Identidad.ObtenerUsuarioActual;

namespace PuntoIOS.UnitTests;

public class ObtenerUsuarioActualHandlerTests
{
    private sealed class FakeAccessor(CurrentUser? user) : ICurrentUserAccessor
    {
        public CurrentUser? GetCurrentUser() => user;
    }

    [Fact]
    public void Devuelve_la_identidad_del_usuario_autenticado()
    {
        var handler = new ObtenerUsuarioActualHandler(
            new FakeAccessor(new CurrentUser("uid-1", "dueno@puntoios.com", true, "Dueño")));

        var resultado = handler.Handle();

        Assert.NotNull(resultado);
        Assert.Equal("uid-1", resultado.Uid);
        Assert.Equal("dueno@puntoios.com", resultado.Email);
        Assert.True(resultado.EmailVerificado);
        Assert.Equal("Dueño", resultado.Nombre);
        Assert.Null(resultado.UsuarioId);
        Assert.Empty(resultado.Permisos);
    }

    [Fact]
    public void Incluye_el_rol_y_los_permisos_del_usuario_local()
    {
        var id = Guid.NewGuid();
        var handler = new ObtenerUsuarioActualHandler(new FakeAccessor(
            new CurrentUser("uid-1", "a@b.com", true, null, id, "Técnico", ["usuarios.ver"])));

        var resultado = handler.Handle();

        Assert.Equal(id, resultado!.UsuarioId);
        Assert.Equal("Técnico", resultado.Rol);
        Assert.Equal(["usuarios.ver"], resultado.Permisos);
    }

    [Fact]
    public void Devuelve_null_si_no_hay_usuario()
    {
        var handler = new ObtenerUsuarioActualHandler(new FakeAccessor(null));

        Assert.Null(handler.Handle());
    }
}
