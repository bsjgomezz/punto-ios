using PuntoIOS.Application.Common.Pagination;
using PuntoIOS.Domain.Clientes;

namespace PuntoIOS.Application.Features.Clientes.Puertos;

/// <summary>Puerto de persistencia de clientes. Siempre opera sobre el local actual.</summary>
public interface IClienteRepositorio
{
    Task<Cliente?> ObtenerPorIdAsync(Guid id, CancellationToken ct);

    Task<IReadOnlyList<Cliente>> ObtenerPorIdsAsync(IEnumerable<Guid> ids, CancellationToken ct);

    /// <summary>Clientes cuyo teléfono normalizado coincide (para detectar duplicados).</summary>
    Task<IReadOnlyList<Cliente>> BuscarPorTelefonoAsync(string telefonoNormalizado, CancellationToken ct);

    /// <summary>Busca por nombre, apellido, email o teléfono. Sin texto devuelve todos. Orden: apellido, nombre.</summary>
    Task<PagedResult<Cliente>> BuscarAsync(string? texto, PageRequest pagina, CancellationToken ct);

    void Agregar(Cliente cliente);

    Task GuardarCambiosAsync(CancellationToken ct);
}
