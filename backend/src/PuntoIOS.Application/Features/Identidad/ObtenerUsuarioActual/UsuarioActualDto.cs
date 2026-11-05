namespace PuntoIOS.Application.Features.Identidad.ObtenerUsuarioActual;

public sealed record UsuarioActualDto(string Uid, string? Email, bool EmailVerificado, string? Nombre);
