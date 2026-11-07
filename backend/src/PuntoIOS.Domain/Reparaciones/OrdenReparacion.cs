namespace PuntoIOS.Domain.Reparaciones;

/// <summary>
/// Orden de reparación: un equipo de un cliente que entra al taller. El número es correlativo y lo asigna la base.
/// El código de desbloqueo se guarda cifrado (lo cifra la capa de aplicación) y se borra al entregar el equipo.
/// </summary>
public sealed class OrdenReparacion
{
    public const int LargoMaximoProblema = 2000;
    public const int LargoMaximoOtroTipo = 200;

    private OrdenReparacion()
    {
    }

    public Guid Id { get; private set; }

    public Guid LocalId { get; private set; }

    /// <summary>Número correlativo para el ticket ("orden 25"). Lo genera una secuencia de PostgreSQL al guardar.</summary>
    public long Numero { get; private set; }

    public Guid ClienteId { get; private set; }

    public Guid DispositivoId { get; private set; }

    /// <summary>Lo que cuenta el cliente, con sus palabras.</summary>
    public string ProblemaInformado { get; private set; } = string.Empty;

    public List<string> TiposReparacion { get; private set; } = [];

    public string? OtroTipoDescripcion { get; private set; }

    public Guid? TecnicoUsuarioId { get; private set; }

    /// <summary>
    /// Fecha orientativa: se calcula con lo que cuenta el cliente y puede cambiar si al revisar el equipo
    /// aparecen otros problemas (por ejemplo, si llegó sin encender).
    /// </summary>
    public DateOnly? FechaEstimadaEntrega { get; private set; }

    public string Estado { get; private set; } = EstadosDeReparacion.Recibido;

    public string? DesbloqueoTipo { get; private set; }

    public string? DesbloqueoCifrado { get; private set; }

    public Guid? CreadoPorUsuarioId { get; private set; }

    public DateTimeOffset CreadoEn { get; private set; }

    public bool TieneDesbloqueo => DesbloqueoCifrado is not null;

    public static OrdenReparacion Recibir(
        Guid localId,
        Guid clienteId,
        Guid dispositivoId,
        string problemaInformado,
        IEnumerable<string> tiposReparacion,
        string? otroTipoDescripcion,
        Guid? tecnicoUsuarioId,
        DateOnly? fechaEstimadaEntrega,
        string? desbloqueoTipo,
        string? desbloqueoCifrado,
        Guid? creadoPorUsuarioId,
        DateTimeOffset ahora)
    {
        if (string.IsNullOrWhiteSpace(problemaInformado))
        {
            throw new ArgumentException("El problema informado es obligatorio.", nameof(problemaInformado));
        }

        var tipos = tiposReparacion.Distinct().ToList();
        if (tipos.Count == 0 || tipos.Any(t => !TiposDeReparacion.Catalogo.ContainsKey(t)))
        {
            throw new ArgumentException("Hay que indicar al menos un tipo de reparación válido.", nameof(tiposReparacion));
        }

        var otro = string.IsNullOrWhiteSpace(otroTipoDescripcion) ? null : otroTipoDescripcion.Trim();
        if (tipos.Contains(TiposDeReparacion.Otro) && otro is null)
        {
            throw new ArgumentException("Falta describir el otro tipo de reparación.", nameof(otroTipoDescripcion));
        }

        if ((desbloqueoTipo is null) != (desbloqueoCifrado is null)
            || (desbloqueoTipo is not null && !TiposDeDesbloqueo.Todos.Contains(desbloqueoTipo)))
        {
            throw new ArgumentException("El desbloqueo no es válido.", nameof(desbloqueoTipo));
        }

        return new OrdenReparacion
        {
            Id = Guid.CreateVersion7(),
            LocalId = localId,
            ClienteId = clienteId,
            DispositivoId = dispositivoId,
            ProblemaInformado = problemaInformado.Trim(),
            TiposReparacion = tipos,
            OtroTipoDescripcion = tipos.Contains(TiposDeReparacion.Otro) ? otro : null,
            TecnicoUsuarioId = tecnicoUsuarioId,
            FechaEstimadaEntrega = fechaEstimadaEntrega,
            Estado = EstadosDeReparacion.Recibido,
            DesbloqueoTipo = desbloqueoTipo,
            DesbloqueoCifrado = desbloqueoCifrado,
            CreadoPorUsuarioId = creadoPorUsuarioId,
            CreadoEn = ahora,
        };
    }

    /// <summary>Borra el código de desbloqueo. Se llama al entregar el equipo (S10).</summary>
    public void BorrarDesbloqueo()
    {
        DesbloqueoTipo = null;
        DesbloqueoCifrado = null;
    }
}
