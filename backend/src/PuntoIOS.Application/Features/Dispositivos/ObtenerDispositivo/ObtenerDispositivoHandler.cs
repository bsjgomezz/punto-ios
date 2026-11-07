using PuntoIOS.Application.Common.Results;
using PuntoIOS.Application.Features.Dispositivos.Puertos;

namespace PuntoIOS.Application.Features.Dispositivos.ObtenerDispositivo;

public sealed class ObtenerDispositivoHandler(IDispositivoRepositorio dispositivos)
{
    public async Task<Resultado<DispositivoDto>> HandleAsync(Guid id, CancellationToken ct)
    {
        var dispositivo = await dispositivos.ObtenerPorIdAsync(id, ct);
        return dispositivo is null
            ? Resultado<DispositivoDto>.Falla(TipoError.NoEncontrado, "dispositivo_inexistente", "El equipo no existe.")
            : Resultado<DispositivoDto>.Ok(DispositivoDto.Desde(dispositivo));
    }
}
