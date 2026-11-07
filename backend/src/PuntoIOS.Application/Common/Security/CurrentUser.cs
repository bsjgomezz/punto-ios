namespace PuntoIOS.Application.Common.Security;

/// <summary>
/// Usuario autenticado de la solicitud actual. Uid, email y nombre vienen de Firebase;
/// UsuarioId, Rol y Permisos vienen del usuario local (null si no está registrado).
/// </summary>
public sealed record CurrentUser(
    string Uid,
    string? Email,
    bool EmailVerified,
    string? Name,
    Guid? UsuarioId = null,
    string? Rol = null,
    IReadOnlyList<string>? Permisos = null);
