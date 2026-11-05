namespace PuntoIOS.Application.Common.Security;

/// <summary>Identidad del usuario autenticado, tal como la informa el proveedor de identidad (Firebase).</summary>
public sealed record CurrentUser(string Uid, string? Email, bool EmailVerified, string? Name);
