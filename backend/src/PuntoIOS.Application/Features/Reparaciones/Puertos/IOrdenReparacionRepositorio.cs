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

    /// <summary>Guarda los cambios. Al guardar una orden nueva, la base le asigna el número.</summary>
    Task GuardarCambiosAsync(CancellationToken ct);
}
