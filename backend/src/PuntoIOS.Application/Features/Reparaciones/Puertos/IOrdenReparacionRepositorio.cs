using PuntoIOS.Application.Common.Pagination;
using PuntoIOS.Domain.Reparaciones;

namespace PuntoIOS.Application.Features.Reparaciones.Puertos;

/// <summary>Puerto de persistencia de órdenes de reparación. Siempre opera sobre el local actual.</summary>
public interface IOrdenReparacionRepositorio
{
    Task<OrdenReparacion?> ObtenerPorIdAsync(Guid id, CancellationToken ct);

    /// <summary>Órdenes de la más nueva a la más vieja, filtradas según <paramref name="filtro"/>.</summary>
    Task<PagedResult<OrdenReparacion>> ListarAsync(FiltroDeOrdenes filtro, PageRequest pagina, CancellationToken ct);

    void Agregar(OrdenReparacion orden);

    void AgregarFoto(FotoOrden foto);

    Task<IReadOnlyList<FotoOrden>> ListarFotosAsync(Guid ordenId, CancellationToken ct);

    Task<FotoOrden?> ObtenerFotoAsync(Guid ordenId, Guid fotoId, CancellationToken ct);

    void AgregarVersionPresupuesto(VersionDePresupuesto version);

    /// <summary>Todas las versiones de presupuesto de la orden, de la más nueva a la más vieja.</summary>
    Task<IReadOnlyList<VersionDePresupuesto>> ListarVersionesPresupuestoAsync(Guid ordenId, CancellationToken ct);

    /// <summary>La versión vigente (número más alto), o null si la orden no tiene ninguna.</summary>
    Task<VersionDePresupuesto?> ObtenerVersionVigentePresupuestoAsync(Guid ordenId, CancellationToken ct);

    /// <summary>La versión vigente de cada orden indicada, en una sola consulta (las órdenes sin presupuesto no figuran).</summary>
    Task<IReadOnlyList<VersionDePresupuesto>> ObtenerVersionesVigentesPresupuestoAsync(IEnumerable<Guid> ordenIds, CancellationToken ct);

    /// <summary>Guarda los cambios. Al guardar una orden nueva, la base le asigna el número.</summary>
    Task GuardarCambiosAsync(CancellationToken ct);
}
