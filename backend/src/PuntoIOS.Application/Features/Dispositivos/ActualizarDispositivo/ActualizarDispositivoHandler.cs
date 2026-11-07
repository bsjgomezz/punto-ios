using PuntoIOS.Application.Common.Multilocal;
using PuntoIOS.Application.Common.Results;
using PuntoIOS.Application.Common.Security;
using PuntoIOS.Application.Features.Auditoria.Puertos;
using PuntoIOS.Application.Features.Clientes.Puertos;
using PuntoIOS.Application.Features.Dispositivos.Puertos;
using PuntoIOS.Domain.Auditoria;
using PuntoIOS.Domain.Clientes;

namespace PuntoIOS.Application.Features.Dispositivos.ActualizarDispositivo;

/// <param name="ActualizarImei">
/// Falso: el IMEI queda como está (la interfaz no lo conoce completo). Verdadero: se reemplaza por
/// <paramref name="ImeiSerie"/> (vacío lo quita).
/// </param>
public sealed record ActualizarDispositivoCommand(
    Guid DispositivoId,
    string? Marca,
    string? Modelo,
    string? Almacenamiento,
    string? Color,
    string? Observaciones,
    bool ActualizarImei,
    string? ImeiSerie);

/// <summary>Edita un equipo y registra qué cambió. El IMEI queda enmascarado en el historial.</summary>
public sealed class ActualizarDispositivoHandler(
    IDispositivoRepositorio dispositivos,
    IClienteRepositorio clientes,
    IAuditoriaRepositorio auditoria,
    ILocalActual local,
    ICurrentUserAccessor usuarioActual,
    TimeProvider reloj)
{
    public async Task<Resultado<DispositivoDto>> HandleAsync(ActualizarDispositivoCommand comando, CancellationToken ct)
    {
        var dispositivo = await dispositivos.ObtenerPorIdAsync(comando.DispositivoId, ct);
        if (dispositivo is null)
        {
            return Resultado<DispositivoDto>.Falla(TipoError.NoEncontrado, "dispositivo_inexistente", "El equipo no existe.");
        }

        var error = DispositivoValidacion.Validar(
            comando.Marca,
            comando.Modelo,
            comando.Almacenamiento,
            comando.Color,
            comando.ActualizarImei ? comando.ImeiSerie : null,
            comando.Observaciones);
        if (error is not null)
        {
            return Resultado<DispositivoDto>.Falla(error);
        }

        var imei = comando.ActualizarImei ? Dispositivo.NormalizarImei(comando.ImeiSerie) : null;
        if (imei is not null
            && imei != dispositivo.ImeiSerie
            && await dispositivos.ObtenerPorImeiAsync(imei, ct) is { } existente
            && existente.Id != dispositivo.Id)
        {
            return Resultado<DispositivoDto>.Falla(
                DispositivoValidacion.ImeiDuplicado(await clientes.ObtenerPorIdAsync(existente.ClienteId, ct)));
        }

        var cambios = dispositivo.Actualizar(
            comando.Marca!,
            comando.Modelo!,
            comando.Almacenamiento,
            comando.Color,
            comando.Observaciones,
            comando.ActualizarImei,
            comando.ImeiSerie);

        if (cambios.Count > 0)
        {
            auditoria.Agregar(RegistroAuditoria.Registrar(
                local.LocalId,
                EntidadesAuditadas.Dispositivo,
                dispositivo.Id,
                AccionesAuditadas.Editado,
                usuarioActual.GetCurrentUser()?.UsuarioId,
                reloj.GetUtcNow(),
                cambios));
            await dispositivos.GuardarCambiosAsync(ct);
        }

        return Resultado<DispositivoDto>.Ok(DispositivoDto.Desde(dispositivo));
    }
}
