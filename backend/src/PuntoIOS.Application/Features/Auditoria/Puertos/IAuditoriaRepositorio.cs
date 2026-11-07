using PuntoIOS.Application.Common.Pagination;
using PuntoIOS.Domain.Auditoria;

namespace PuntoIOS.Application.Features.Auditoria.Puertos;

public interface IAuditoriaRepositorio
{
    /// <summary>Se guarda junto con el cambio auditado en el próximo <c>GuardarCambiosAsync</c> del repositorio de la entidad.</summary>
    void Agregar(RegistroAuditoria registro);

    /// <summary>Historial de una entidad, del más reciente al más antiguo.</summary>
    Task<PagedResult<RegistroAuditoria>> ListarAsync(string entidad, Guid entidadId, PageRequest pagina, CancellationToken ct);
}
