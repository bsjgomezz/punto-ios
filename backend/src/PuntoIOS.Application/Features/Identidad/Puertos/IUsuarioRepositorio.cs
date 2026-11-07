using PuntoIOS.Application.Common.Pagination;
using PuntoIOS.Domain.Identidad;

namespace PuntoIOS.Application.Features.Identidad.Puertos;

/// <summary>Puerto de persistencia de usuarios. Los usuarios se devuelven con su rol y permisos cargados.</summary>
public interface IUsuarioRepositorio
{
    Task<Usuario?> ObtenerPorFirebaseUidAsync(string uid, CancellationToken ct);

    Task<Usuario?> ObtenerPorEmailAsync(string emailNormalizado, CancellationToken ct);

    Task<Usuario?> ObtenerPorIdAsync(Guid id, CancellationToken ct);

    Task<PagedResult<Usuario>> ListarAsync(PageRequest pagina, CancellationToken ct);

    /// <summary>Cantidad de usuarios activos cuyo rol concede el permiso indicado.</summary>
    Task<int> ContarActivosConPermisoAsync(string permiso, CancellationToken ct);

    void Agregar(Usuario usuario);

    Task GuardarCambiosAsync(CancellationToken ct);
}
