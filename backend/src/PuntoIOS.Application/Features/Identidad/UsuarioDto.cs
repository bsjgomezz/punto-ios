using PuntoIOS.Domain.Identidad;

namespace PuntoIOS.Application.Features.Identidad;

public sealed record UsuarioDto(
    Guid Id,
    string Email,
    string Nombre,
    bool Activo,
    Guid RolId,
    string Rol,
    bool Vinculado,
    DateTimeOffset CreadoEn,
    DateTimeOffset? UltimoIngresoEn)
{
    public static UsuarioDto Desde(Usuario u) =>
        new(u.Id, u.Email, u.Nombre, u.Activo, u.RolId, u.Rol.Nombre, u.FirebaseUid is not null, u.CreadoEn, u.UltimoIngresoEn);
}

public sealed record RolDto(Guid Id, string Nombre, string Descripcion, bool EsDeSistema, IReadOnlyList<string> Permisos)
{
    public static RolDto Desde(Rol r) => new(r.Id, r.Nombre, r.Descripcion, r.EsDeSistema, r.CodigosDePermisos());
}
