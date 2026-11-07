namespace PuntoIOS.Domain.Identidad;

/// <summary>Relación entre un rol y un permiso que ese rol concede.</summary>
public sealed class RolPermiso
{
    private RolPermiso()
    {
    }

    public RolPermiso(Guid rolId, string permisoCodigo)
    {
        RolId = rolId;
        PermisoCodigo = permisoCodigo;
    }

    public Guid RolId { get; private set; }

    public string PermisoCodigo { get; private set; } = string.Empty;
}
