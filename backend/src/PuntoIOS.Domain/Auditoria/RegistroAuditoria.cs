namespace PuntoIOS.Domain.Auditoria;

/// <summary>Un campo que cambió: valor anterior y valor nuevo (null = vacío).</summary>
public sealed record CambioDeCampo(string Campo, string? Anterior, string? Nuevo);

public static class EntidadesAuditadas
{
    public const string Cliente = "cliente";
    public const string Dispositivo = "dispositivo";
}

public static class AccionesAuditadas
{
    public const string Creado = "creado";
    public const string Editado = "editado";

    /// <summary>Alguien vio el IMEI o número de serie completo de un equipo.</summary>
    public const string ImeiConsultado = "imei_consultado";
}

/// <summary>
/// Registro inmutable de quién hizo qué y cuándo sobre una entidad. Se guarda en la misma transacción
/// que el cambio auditado, así nunca hay cambios sin registro.
/// </summary>
public sealed class RegistroAuditoria
{
    private RegistroAuditoria()
    {
    }

    public Guid Id { get; private set; }

    public Guid LocalId { get; private set; }

    public string Entidad { get; private set; } = string.Empty;

    public Guid EntidadId { get; private set; }

    public string Accion { get; private set; } = string.Empty;

    public Guid? UsuarioId { get; private set; }

    public DateTimeOffset Fecha { get; private set; }

    public List<CambioDeCampo> Cambios { get; private set; } = [];

    public static RegistroAuditoria Registrar(
        Guid localId,
        string entidad,
        Guid entidadId,
        string accion,
        Guid? usuarioId,
        DateTimeOffset fecha,
        IEnumerable<CambioDeCampo>? cambios = null) =>
        new()
        {
            Id = Guid.CreateVersion7(),
            LocalId = localId,
            Entidad = entidad,
            EntidadId = entidadId,
            Accion = accion,
            UsuarioId = usuarioId,
            Fecha = fecha,
            Cambios = cambios?.ToList() ?? [],
        };
}
