using PuntoIOS.Domain.Auditoria;

namespace PuntoIOS.Domain.Reparaciones;

/// <summary>
/// Orden de reparación: un equipo de un cliente que entra al taller. El número es correlativo y lo asigna la base.
/// El código de desbloqueo se guarda cifrado (lo cifra la capa de aplicación) y se borra al entregar el equipo.
/// </summary>
public sealed class OrdenReparacion
{
    public const int LargoMaximoProblema = 2000;
    public const int LargoMaximoOtroTipo = 200;
    public const int LargoMaximoTextoTecnico = 2000;
    public const int LargoMaximoComponenteAfectado = 60;
    public const int MaximoComponentesAfectados = 20;

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

    /// <summary>Ítems revisados al recibir el equipo (los no revisados no figuran).</summary>
    public List<ItemChecklist> ChecklistItems { get; private set; } = [];

    /// <summary>Salud de la batería en % (0 a 100), si se pudo ver.</summary>
    public int? SaludBateria { get; private set; }

    public string? NotasChecklist { get; private set; }

    /// <summary>Pruebas que se le hicieron al equipo. Lo completa quien trabaja en la reparación.</summary>
    public string? Pruebas { get; private set; }

    /// <summary>Lo que se determinó al revisar el equipo.</summary>
    public string? Diagnostico { get; private set; }

    /// <summary>Mediciones tomadas (voltajes, consumos, etc.).</summary>
    public string? Mediciones { get; private set; }

    /// <summary>
    /// Etiquetas libres de los componentes afectados ("Pantalla", "Conector de carga", "IC de carga").
    /// No es un catálogo fijo: las escribe quien repara. Sin repetidos y en el orden en que se cargaron.
    /// </summary>
    public List<string> ComponentesAfectados { get; private set; } = [];

    /// <summary>
    /// Notas privadas del taller. NUNCA se expone en el portal público de seguimiento del cliente (S18) ni en
    /// ningún otro DTO de cara al cliente — si en el futuro agregás un DTO público para una orden, no incluyas este campo.
    /// </summary>
    public string? NotasInternas { get; private set; }

    public bool TieneDesbloqueo => DesbloqueoCifrado is not null;

    /// <summary>Llegó sin encender: no se puede determinar la falla completa ni una fecha estimada.</summary>
    public bool NoEnciende => ChecklistItems.Any(i => i.Codigo == ItemsDeChecklist.Enciende && !i.Valor);

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
        DateTimeOffset ahora,
        ChecklistIngreso? checklist = null)
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

        var items = checklist?.Items.ToList() ?? [];
        if (items.Any(i => !ItemsDeChecklist.Existe(i.Codigo)) || items.Select(i => i.Codigo).Distinct().Count() != items.Count)
        {
            throw new ArgumentException("El checklist tiene ítems inválidos o repetidos.", nameof(checklist));
        }

        if (checklist?.SaludBateria is < 0 or > 100)
        {
            throw new ArgumentException("La salud de la batería va de 0 a 100.", nameof(checklist));
        }

        var notas = checklist?.Notas?.Trim() is { Length: > 0 } texto ? texto : null;
        if (notas?.Length > ItemsDeChecklist.LargoMaximoNotas)
        {
            throw new ArgumentException("Las notas del checklist son demasiado largas.", nameof(checklist));
        }

        var noEnciende = items.Any(i => i.Codigo == ItemsDeChecklist.Enciende && !i.Valor);
        if (noEnciende && fechaEstimadaEntrega is not null)
        {
            throw new ArgumentException(ItemsDeChecklist.RazonSinFecha, nameof(fechaEstimadaEntrega));
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
            ChecklistItems = items,
            SaludBateria = checklist?.SaludBateria,
            NotasChecklist = notas,
        };
    }

    /// <summary>
    /// Pasa la orden a otro estado si la transición está permitida (<see cref="MaquinaDeEstados"/>).
    /// Al entregar el equipo se borra el código de desbloqueo. Quién y cuándo lo registra la auditoría.
    /// </summary>
    public void CambiarEstado(string nuevoEstado)
    {
        if (nuevoEstado is null || !EstadosDeReparacion.Catalogo.ContainsKey(nuevoEstado))
        {
            throw new ArgumentException("El estado no existe.", nameof(nuevoEstado));
        }

        if (!MaquinaDeEstados.PuedeTransicionar(Estado, nuevoEstado))
        {
            throw new ArgumentException($"No se puede pasar de \"{Estado}\" a \"{nuevoEstado}\".", nameof(nuevoEstado));
        }

        Estado = nuevoEstado;
        if (nuevoEstado == EstadosDeReparacion.Entregado)
        {
            BorrarDesbloqueo();
        }
    }

    /// <summary>
    /// Reemplaza los datos técnicos y devuelve únicamente los campos que realmente cambiaron.
    /// Los textos vacíos quedan en null; los componentes se recortan, se descartan los vacíos y se quitan los
    /// repetidos (queda la primera aparición). Valida todo antes de tocar nada: si algo no es válido no cambia ningún campo.
    /// En el historial, una lista de componentes vacía se registra como null (vacío).
    /// </summary>
    public IReadOnlyList<CambioDeCampo> ActualizarDatosTecnicos(
        string? pruebas,
        string? diagnostico,
        string? mediciones,
        List<string>? componentesAfectados,
        string? notasInternas)
    {
        var pruebasNuevas = TextoTecnico(pruebas, "Las pruebas admiten", nameof(pruebas));
        var diagnosticoNuevo = TextoTecnico(diagnostico, "El diagnóstico admite", nameof(diagnostico));
        var medicionesNuevas = TextoTecnico(mediciones, "Las mediciones admiten", nameof(mediciones));
        var notasInternasNuevas = TextoTecnico(notasInternas, "Las notas internas admiten", nameof(notasInternas));

        var componentesNuevos = (componentesAfectados ?? [])
            .Select(VacioANulo)
            .OfType<string>()
            .Distinct()
            .ToList();
        if (componentesNuevos.Any(c => c.Length > LargoMaximoComponenteAfectado))
        {
            throw new ArgumentException(
                $"Cada componente afectado admite hasta {LargoMaximoComponenteAfectado} caracteres.", nameof(componentesAfectados));
        }

        if (componentesNuevos.Count > MaximoComponentesAfectados)
        {
            throw new ArgumentException(
                $"Se pueden cargar hasta {MaximoComponentesAfectados} componentes afectados.", nameof(componentesAfectados));
        }

        var cambios = new List<CambioDeCampo>();

        if (pruebasNuevas != Pruebas)
        {
            cambios.Add(new CambioDeCampo("pruebas", Pruebas, pruebasNuevas));
            Pruebas = pruebasNuevas;
        }

        if (diagnosticoNuevo != Diagnostico)
        {
            cambios.Add(new CambioDeCampo("diagnostico", Diagnostico, diagnosticoNuevo));
            Diagnostico = diagnosticoNuevo;
        }

        if (medicionesNuevas != Mediciones)
        {
            cambios.Add(new CambioDeCampo("mediciones", Mediciones, medicionesNuevas));
            Mediciones = medicionesNuevas;
        }

        if (!componentesNuevos.SequenceEqual(ComponentesAfectados))
        {
            cambios.Add(new CambioDeCampo(
                "componentes_afectados", UnirComponentes(ComponentesAfectados), UnirComponentes(componentesNuevos)));
            ComponentesAfectados = componentesNuevos;
        }

        if (notasInternasNuevas != NotasInternas)
        {
            cambios.Add(new CambioDeCampo("notas_internas", NotasInternas, notasInternasNuevas));
            NotasInternas = notasInternasNuevas;
        }

        return cambios;
    }

    /// <summary>Borra el código de desbloqueo. Se llama al entregar el equipo.</summary>
    public void BorrarDesbloqueo()
    {
        DesbloqueoTipo = null;
        DesbloqueoCifrado = null;
    }

    private static string? TextoTecnico(string? valor, string inicioDelMensaje, string parametro)
    {
        var texto = VacioANulo(valor);
        if (texto?.Length > LargoMaximoTextoTecnico)
        {
            throw new ArgumentException($"{inicioDelMensaje} hasta {LargoMaximoTextoTecnico} caracteres.", parametro);
        }

        return texto;
    }

    /// <summary>Texto para el historial: "Pantalla, Conector de carga", o null si no hay ninguno.</summary>
    private static string? UnirComponentes(List<string> componentes) =>
        componentes.Count == 0 ? null : string.Join(", ", componentes);

    private static string? VacioANulo(string? valor) => string.IsNullOrWhiteSpace(valor) ? null : valor.Trim();
}
