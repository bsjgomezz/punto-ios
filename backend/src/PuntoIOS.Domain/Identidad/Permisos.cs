namespace PuntoIOS.Domain.Identidad;

/// <summary>
/// Catálogo de permisos del sistema. Cada fase agrega los suyos (con su migración).
/// El código del permiso es el que viaja en las políticas de autorización.
/// </summary>
public static class Permisos
{
    public const string UsuariosVer = "usuarios.ver";
    public const string UsuariosGestionar = "usuarios.gestionar";
    public const string RolesVer = "roles.ver";
    public const string ClientesVer = "clientes.ver";
    public const string ClientesCrear = "clientes.crear";

    /// <summary>Todos los permisos con su descripción (se siembran en la base de datos).</summary>
    public static readonly IReadOnlyDictionary<string, string> Catalogo = new Dictionary<string, string>
    {
        [UsuariosVer] = "Ver usuarios del sistema",
        [UsuariosGestionar] = "Registrar usuarios, cambiar su rol y activarlos o desactivarlos",
        [RolesVer] = "Ver roles y sus permisos",
        [ClientesVer] = "Ver y buscar clientes",
        [ClientesCrear] = "Registrar clientes nuevos",
    };
}
