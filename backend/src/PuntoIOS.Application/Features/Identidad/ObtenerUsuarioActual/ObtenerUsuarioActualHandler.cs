using PuntoIOS.Application.Common.Security;

namespace PuntoIOS.Application.Features.Identidad.ObtenerUsuarioActual;

/// <summary>Caso de uso: devuelve la identidad del usuario autenticado en la solicitud actual.</summary>
public sealed class ObtenerUsuarioActualHandler(ICurrentUserAccessor currentUser)
{
    public UsuarioActualDto? Handle()
    {
        var user = currentUser.GetCurrentUser();
        return user is null
            ? null
            : new UsuarioActualDto(user.Uid, user.Email, user.EmailVerified, user.Name);
    }
}
