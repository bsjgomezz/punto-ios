using PuntoIOS.Application.Common.Multilocal;
using PuntoIOS.Application.Common.Results;
using PuntoIOS.Application.Common.Security;
using PuntoIOS.Application.Features.Auditoria.Puertos;
using PuntoIOS.Application.Features.Dispositivos.Puertos;
using PuntoIOS.Domain.Auditoria;

namespace PuntoIOS.Application.Features.Dispositivos.RevelarImei;

/// <summary>Devuelve el IMEI completo y deja registrado quién lo vio y cuándo.</summary>
public sealed class RevelarImeiHandler(
    IDispositivoRepositorio dispositivos,
    IAuditoriaRepositorio auditoria,
    ILocalActual local,
    ICurrentUserAccessor usuarioActual,
    TimeProvider reloj)
{
    public async Task<Resultado<ImeiDto>> HandleAsync(Guid dispositivoId, CancellationToken ct)
    {
        var dispositivo = await dispositivos.ObtenerPorIdAsync(dispositivoId, ct);
        if (dispositivo is null)
        {
            return Resultado<ImeiDto>.Falla(TipoError.NoEncontrado, "dispositivo_inexistente", "El equipo no existe.");
        }

        if (dispositivo.ImeiSerie is not null)
        {
            auditoria.Agregar(RegistroAuditoria.Registrar(
                local.LocalId,
                EntidadesAuditadas.Dispositivo,
                dispositivo.Id,
                AccionesAuditadas.ImeiConsultado,
                usuarioActual.GetCurrentUser()?.UsuarioId,
                reloj.GetUtcNow()));
            await dispositivos.GuardarCambiosAsync(ct);
        }

        return Resultado<ImeiDto>.Ok(new ImeiDto(dispositivo.ImeiSerie));
    }
}
