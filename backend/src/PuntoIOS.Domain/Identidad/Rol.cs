namespace PuntoIOS.Domain.Identidad;

/// <summary>Rol configurable: agrupa permisos. Los roles de sistema no se pueden eliminar.</summary>
public sealed class Rol
{
    private readonly List<RolPermiso> _permisos = [];

    private Rol()
    {
    }

    public Rol(Guid id, string nombre, string descripcion, bool esDeSistema)
    {
        Id = id;
        Nombre = nombre;
        Descripcion = descripcion;
        EsDeSistema = esDeSistema;
    }

    public Guid Id { get; private set; }

    public string Nombre { get; private set; } = string.Empty;

    public string Descripcion { get; private set; } = string.Empty;

    public bool EsDeSistema { get; private set; }

    public IReadOnlyCollection<RolPermiso> Permisos => _permisos;

    public void Conceder(string permiso)
    {
        if (!Concede(permiso))
        {
            _permisos.Add(new RolPermiso(Id, permiso));
        }
    }

    public bool Concede(string permiso) => _permisos.Any(p => p.PermisoCodigo == permiso);

    public IReadOnlyList<string> CodigosDePermisos() =>
        _permisos.Select(p => p.PermisoCodigo).OrderBy(c => c, StringComparer.Ordinal).ToList();
}
