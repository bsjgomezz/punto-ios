using PuntoIOS.Application.Common.Results;
using PuntoIOS.Application.Features.Identidad.Puertos;
using PuntoIOS.Application.Features.Reparaciones.Puertos;

namespace PuntoIOS.Application.Features.Reparaciones.ListarVersionesPresupuesto;

/// <summary>Todas las versiones de presupuesto de la orden, de la más nueva a la más vieja, con el nombre de quién las creó.</summary>
public sealed class ListarVersionesPresupuestoHandler(IOrdenReparacionRepositorio ordenes, IDirectorioDeUsuarios directorio)
{
    public async Task<Resultado<IReadOnlyList<VersionDePresupuestoDto>>> HandleAsync(Guid ordenId, CancellationToken ct)
    {
        if (await ordenes.ObtenerPorIdAsync(ordenId, ct) is null)
        {
            return Resultado<IReadOnlyList<VersionDePresupuestoDto>>.Falla(
                TipoError.NoEncontrado, "orden_inexistente", "La orden no existe.");
        }

        var versiones = await ordenes.ListarVersionesPresupuestoAsync(ordenId, ct);
        var nombres = await directorio.ObtenerNombresAsync(
            versiones.Where(v => v.CreadoPorUsuarioId is not null).Select(v => v.CreadoPorUsuarioId!.Value).Distinct(), ct);
        return Resultado<IReadOnlyList<VersionDePresupuestoDto>>.Ok(
            versiones.Select(v => VersionDePresupuestoDto.Desde(v, nombres)).ToList());
    }
}
