using PuntoIOS.Application.Common.Security;

namespace PuntoIOS.Api.Authentication;

/// <summary>Adaptador: arma el usuario actual a partir de los claims del token ya validado y del usuario local identificado.</summary>
public sealed class HttpContextCurrentUserAccessor(IHttpContextAccessor httpContextAccessor) : ICurrentUserAccessor
{
    public CurrentUser? GetCurrentUser()
    {
        var principal = httpContextAccessor.HttpContext?.User;
        return principal?.Identity?.IsAuthenticated == true ? FirebaseClaims.Leer(principal) : null;
    }
}
