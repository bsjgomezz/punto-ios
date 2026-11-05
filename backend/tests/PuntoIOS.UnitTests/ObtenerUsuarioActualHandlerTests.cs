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

        Assert.Equal(new UsuarioActualDto("uid-1", "dueno@puntoios.com", true, "Dueño"), resultado);
    }

    [Fact]
    public void Devuelve_null_si_no_hay_usuario()
    {
        var handler = new ObtenerUsuarioActualHandler(new FakeAccessor(null));

        Assert.Null(handler.Handle());
    }
}
