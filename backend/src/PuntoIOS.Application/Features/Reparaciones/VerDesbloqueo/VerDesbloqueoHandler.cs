using PuntoIOS.Application.Common.Multilocal;
using PuntoIOS.Application.Common.Results;
using PuntoIOS.Application.Common.Security;
using PuntoIOS.Application.Features.Auditoria.Puertos;
using PuntoIOS.Application.Features.Reparaciones.Puertos;
using PuntoIOS.Domain.Auditoria;

namespace PuntoIOS.Application.Features.Reparaciones.VerDesbloqueo;

public sealed record DesbloqueoDto(string Tipo, string Valor);

/// <summary>Devuelve el código o patrón de desbloqueo y deja registrado quién lo vio y cuándo.</summary>
public sealed class VerDesbloqueoHandler(
    IOrdenReparacionRepositorio ordenes,
    IAuditoriaRepositorio auditoria,
    ICifradoDeDatos cifrado,
    ILocalActual local,
    ICurrentUserAccessor usuarioActual,
    TimeProvider reloj)
{
    public async Task<Resultado<DesbloqueoDto>> HandleAsync(Guid ordenId, CancellationToken ct)
    {
        var orden = await ordenes.ObtenerPorIdAsync(ordenId, ct);
        if (orden is null)
        {
            return Resultado<DesbloqueoDto>.Falla(TipoError.NoEncontrado, "orden_inexistente", "La orden no existe.");
        }

        if (orden.DesbloqueoTipo is null || orden.DesbloqueoCifrado is null)
        {
            return Resultado<DesbloqueoDto>.Falla(
                TipoError.NoEncontrado, "sin_desbloqueo", "Esta orden no tiene código de desbloqueo guardado.");
        }

        var valor = cifrado.Descifrar(orden.DesbloqueoCifrado);
        auditoria.Agregar(RegistroAuditoria.Registrar(
            local.LocalId,
            EntidadesAuditadas.Reparacion,
            orden.Id,
            AccionesAuditadas.DesbloqueoConsultado,
            usuarioActual.GetCurrentUser()?.UsuarioId,
            reloj.GetUtcNow()));
        await ordenes.GuardarCambiosAsync(ct);

        return Resultado<DesbloqueoDto>.Ok(new DesbloqueoDto(orden.DesbloqueoTipo, valor));
    }
}
