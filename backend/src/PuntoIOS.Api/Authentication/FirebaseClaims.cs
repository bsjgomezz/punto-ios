using System.Security.Claims;
using PuntoIOS.Application.Common.Security;

namespace PuntoIOS.Api.Authentication;

/// <summary>Nombres de claims y lectura del usuario a partir del principal de la solicitud.</summary>
public static class FirebaseClaims
{
    // Claims agregados por el backend luego de identificar al usuario local (no vienen en el token).
    public const string UsuarioId = "puntoios:usuario_id";
    public const string Rol = "puntoios:rol";
    public const string Permiso = "puntoios:permiso";

    public static CurrentUser? Leer(ClaimsPrincipal? principal)
    {
        var uid = principal?.FindFirst("sub")?.Value;
        if (principal is null || string.IsNullOrWhiteSpace(uid))
        {
            return null;
        }

        var emailVerified = string.Equals(
            principal.FindFirst("email_verified")?.Value,
            "true",
            StringComparison.OrdinalIgnoreCase);

        Guid? usuarioId = Guid.TryParse(principal.FindFirst(UsuarioId)?.Value, out var id) ? id : null;
        var permisos = principal.FindAll(Permiso).Select(c => c.Value).OrderBy(p => p, StringComparer.Ordinal).ToList();

        return new CurrentUser(
            uid,
            principal.FindFirst("email")?.Value,
            emailVerified,
            principal.FindFirst("name")?.Value,
            usuarioId,
            principal.FindFirst(Rol)?.Value,
            permisos);
    }
}
