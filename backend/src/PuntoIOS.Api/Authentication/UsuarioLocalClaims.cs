using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using PuntoIOS.Application.Features.Identidad.IdentificarUsuario;

namespace PuntoIOS.Api.Authentication;

/// <summary>
/// Se ejecuta cuando el Firebase ID Token ya fue validado: busca al usuario local y le agrega rol y permisos
/// como claims. Si no está registrado o está desactivado no se agregan, y las políticas responden 403.
/// </summary>
public static class UsuarioLocalClaims
{
    public static async Task AsociarAsync(TokenValidatedContext context)
    {
        var identidad = FirebaseClaims.Leer(context.Principal);
        var claimsIdentity = context.Principal?.Identities.FirstOrDefault();
        if (identidad is null || claimsIdentity is null)
        {
            return;
        }

        // Defensa: estos claims solo los puede agregar el backend.
        foreach (var claim in claimsIdentity.Claims
                     .Where(c => c.Type is FirebaseClaims.UsuarioId or FirebaseClaims.Rol or FirebaseClaims.Permiso)
                     .ToList())
        {
            claimsIdentity.RemoveClaim(claim);
        }

        var handler = context.HttpContext.RequestServices.GetRequiredService<IdentificarUsuarioHandler>();
        var resultado = await handler.HandleAsync(identidad, context.HttpContext.RequestAborted);

        if (resultado.Estado != EstadoIdentificacion.Autorizado || resultado.Usuario is null)
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>()
                .CreateLogger("PuntoIOS.Autenticacion");
            logger.LogWarning("Usuario de Firebase {Uid} sin acceso: {Estado}", identidad.Uid, resultado.Estado);
            return;
        }

        var usuario = resultado.Usuario;
        claimsIdentity.AddClaim(new Claim(FirebaseClaims.UsuarioId, usuario.Id.ToString()));
        claimsIdentity.AddClaim(new Claim(FirebaseClaims.Rol, usuario.Rol));
        foreach (var permiso in usuario.Permisos)
        {
            claimsIdentity.AddClaim(new Claim(FirebaseClaims.Permiso, permiso));
        }
    }
}
