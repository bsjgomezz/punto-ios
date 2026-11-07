using PuntoIOS.Application.Common.Pagination;
using PuntoIOS.Application.Common.Results;
using PuntoIOS.Application.Features.Auditoria;
using PuntoIOS.Application.Features.Auditoria.Puertos;
using PuntoIOS.Application.Features.Dispositivos.Puertos;
using PuntoIOS.Application.Features.Identidad.Puertos;
using PuntoIOS.Domain.Auditoria;

namespace PuntoIOS.Application.Features.Dispositivos.HistorialDispositivo;

public sealed class HistorialDispositivoHandler(
    IDispositivoRepositorio dispositivos,
    IAuditoriaRepositorio auditoria,
    IDirectorioDeUsuarios directorio)
{
    public async Task<Resultado<PagedResult<RegistroHistorialDto>>> HandleAsync(
        Guid dispositivoId, PageRequest pagina, CancellationToken ct)
    {
        if (await dispositivos.ObtenerPorIdAsync(dispositivoId, ct) is null)
        {
            return Resultado<PagedResult<RegistroHistorialDto>>.Falla(
                TipoError.NoEncontrado, "dispositivo_inexistente", "El equipo no existe.");
        }

        var registros = await auditoria.ListarAsync(EntidadesAuditadas.Dispositivo, dispositivoId, pagina, ct);
        return Resultado<PagedResult<RegistroHistorialDto>>.Ok(await HistorialDeAuditoria.ArmarAsync(registros, directorio, ct));
    }
}
