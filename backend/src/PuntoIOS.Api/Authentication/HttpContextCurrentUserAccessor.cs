using PuntoIOS.Application.Common.Security;

namespace PuntoIOS.Api.Authentication;

/// <summary>Adaptador: arma el usuario actual a partir de los claims del Firebase ID Token ya validado.</summary>
public sealed class HttpContextCurrentUserAccessor(IHttpContextAccessor httpContextAccessor) : ICurrentUserAccessor
{
    public CurrentUser? GetCurrentUser()
    {
        var principal = httpContextAccessor.HttpContext?.User;
        if (principal?.Identity?.IsAuthenticated != true)
        {
            return null;
        }

        var uid = principal.FindFirst("sub")?.Value;
        if (string.IsNullOrWhiteSpace(uid))
        {
            return null;
        }

        var emailVerified = string.Equals(
            principal.FindFirst("email_verified")?.Value,
            "true",
            StringComparison.OrdinalIgnoreCase);

        return new CurrentUser(
            uid,
            principal.FindFirst("email")?.Value,
            emailVerified,
            principal.FindFirst("name")?.Value);
    }
}
