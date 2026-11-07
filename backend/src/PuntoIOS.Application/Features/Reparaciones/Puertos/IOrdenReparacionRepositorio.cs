using PuntoIOS.Application.Common.Pagination;
using PuntoIOS.Domain.Reparaciones;

namespace PuntoIOS.Application.Features.Reparaciones.Puertos;

/// <summary>Puerto de persistencia de órdenes de reparación. Siempre opera sobre el local actual.</summary>
public interface IOrdenReparacionRepositorio
{
    Task<OrdenReparacion?> ObtenerPorIdAsync(Guid id, CancellationToken ct);

    /// <summary>Órdenes de la más nueva a la más vieja; opcionalmente solo las de un cliente.</summary>
    Task<PagedResult<OrdenReparacion>> ListarAsync(Guid? clienteId, PageRequest pagina, CancellationToken ct);

    void Agregar(OrdenReparacion orden);

    /// <summary>Guarda los cambios. Al guardar una orden nueva, la base le asigna el número.</summary>
    Task GuardarCambiosAsync(CancellationToken ct);
}
