namespace PuntoIOS.Domain.Identidad;

/// <summary>Permiso del catálogo (ver <see cref="Permisos"/>).</summary>
public sealed class Permiso
{
    private Permiso()
    {
    }

    public Permiso(string codigo, string descripcion)
    {
        Codigo = codigo;
        Descripcion = descripcion;
    }

    public string Codigo { get; private set; } = string.Empty;

    public string Descripcion { get; private set; } = string.Empty;
}
