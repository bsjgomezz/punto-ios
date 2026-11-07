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
    public const string ClientesEditar = "clientes.editar";
    public const string ClientesSeguimiento = "clientes.seguimiento";
    public const string DispositivosVer = "dispositivos.ver";
    public const string DispositivosCrear = "dispositivos.crear";
    public const string DispositivosEditar = "dispositivos.editar";
    public const string DispositivosVerImei = "dispositivos.ver_imei";
    public const string ReparacionesVer = "reparaciones.ver";
    public const string ReparacionesCrear = "reparaciones.crear";
    public const string ReparacionesReparar = "reparaciones.reparar";
    public const string ReparacionesVerDesbloqueo = "reparaciones.ver_desbloqueo";

    /// <summary>Todos los permisos con su descripción (se siembran en la base de datos).</summary>
    public static readonly IReadOnlyDictionary<string, string> Catalogo = new Dictionary<string, string>
    {
        [UsuariosVer] = "Ver usuarios del sistema",
        [UsuariosGestionar] = "Registrar usuarios, cambiar su rol y activarlos o desactivarlos",
        [RolesVer] = "Ver roles y sus permisos",
        [ClientesVer] = "Ver y buscar clientes",
        [ClientesCrear] = "Registrar clientes nuevos",
        [ClientesEditar] = "Editar los datos de un cliente",
        [ClientesSeguimiento] = "Ver, enviar y regenerar el código de seguimiento del cliente (queda registrado)",
        [DispositivosVer] = "Ver los equipos de los clientes (IMEI enmascarado)",
        [DispositivosCrear] = "Registrar equipos de clientes",
        [DispositivosEditar] = "Editar los datos de un equipo",
        [DispositivosVerImei] = "Ver el IMEI o número de serie completo (queda registrado)",
        [ReparacionesVer] = "Ver las órdenes de reparación",
        [ReparacionesCrear] = "Recibir equipos y crear órdenes de reparación",
        [ReparacionesReparar] = "Trabajar en reparaciones (se le pueden asignar órdenes)",
        [ReparacionesVerDesbloqueo] = "Ver el código o patrón de desbloqueo del equipo (queda registrado)",
    };
}
