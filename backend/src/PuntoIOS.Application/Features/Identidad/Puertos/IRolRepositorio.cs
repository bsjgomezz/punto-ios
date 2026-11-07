using PuntoIOS.Domain.Identidad;

namespace PuntoIOS.Application.Features.Identidad.Puertos;

public interface IRolRepositorio
{
    Task<IReadOnlyList<Rol>> ListarAsync(CancellationToken ct);

    Task<Rol?> ObtenerPorIdAsync(Guid id, CancellationToken ct);
}
