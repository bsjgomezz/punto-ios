using static PuntoIOS.Domain.Reparaciones.EstadosDeReparacion;

namespace PuntoIOS.Domain.Reparaciones;

/// <summary>
/// Transiciones permitidas entre estados de una orden (SDD, sección 6). Todo lo que no figura acá se rechaza,
/// incluido quedarse en el mismo estado. "Entregado" y "Cancelado" son terminales.
/// </summary>
public static class MaquinaDeEstados
{
    public static readonly IReadOnlyDictionary<string, string[]> Transiciones = new Dictionary<string, string[]>
    {
        [Recibido] = [EnPruebas, Cancelado],
        [EnPruebas] = [EnDiagnostico, Cancelado],
        [EnDiagnostico] = [EsperandoPresupuesto, Irreparable, Cancelado],
        [EsperandoPresupuesto] = [PresupuestoEnviado, Cancelado],
        [PresupuestoEnviado] = [EsperandoAprobacion],
        [EsperandoAprobacion] = [Aprobado, PresupuestoRechazado],
        [PresupuestoRechazado] = [EsperandoPresupuesto, Entregado, Cancelado],
        [Aprobado] = [EnReparacion, EsperandoAprobacion, Cancelado],
        [EnReparacion] = [EsperandoRepuesto, ReparacionCompletada, Irreparable, EsperandoAprobacion],
        [EsperandoRepuesto] = [EnReparacion, Cancelado],
        [ReparacionCompletada] = [ListoParaRetirar, EnReparacion],
        [ListoParaRetirar] = [Entregado],
        [Irreparable] = [Entregado],
        [Entregado] = [],
        [Cancelado] = [],
    };

    public static bool PuedeTransicionar(string desde, string hacia) =>
        Transiciones.TryGetValue(desde, out var permitidos) && permitidos.Contains(hacia);

    /// <summary>Estados a los que se puede pasar desde <paramref name="desde"/> (vacío si es terminal o no existe).</summary>
    public static IReadOnlyList<string> SiguientesEstados(string desde) =>
        Transiciones.TryGetValue(desde, out var permitidos) ? permitidos : [];
}
