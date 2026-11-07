namespace PuntoIOS.Domain.Identidad;

/// <summary>Roles que vienen con el sistema. Los identificadores son fijos para poder sembrarlos y referenciarlos.</summary>
public static class RolesDeSistema
{
    public static readonly Guid AdministradorId = new("0195a000-0000-7000-8000-000000000001");
    public static readonly Guid RecepcionVentasId = new("0195a000-0000-7000-8000-000000000002");
    public static readonly Guid TecnicoId = new("0195a000-0000-7000-8000-000000000003");

    public const string Administrador = "Administrador";
    public const string RecepcionVentas = "Recepción/Ventas";
    public const string Tecnico = "Técnico";
}
