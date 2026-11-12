using System.Globalization;
using PuntoIOS.Application.Common.Multilocal;
using PuntoIOS.Application.Common.Results;
using PuntoIOS.Application.Common.Security;
using PuntoIOS.Application.Features.Auditoria.Puertos;
using PuntoIOS.Application.Features.Clientes.Puertos;
using PuntoIOS.Application.Features.Dispositivos.Puertos;
using PuntoIOS.Application.Features.Identidad.Puertos;
using PuntoIOS.Application.Features.Reparaciones.Puertos;
using PuntoIOS.Domain.Auditoria;
using PuntoIOS.Domain.Reparaciones;

namespace PuntoIOS.Application.Features.Reparaciones.ResponderPresupuesto;

public sealed record ResponderPresupuestoCommand(Guid OrdenId, bool Aprobado);

/// <summary>
/// Registra la respuesta (aprobado o rechazado) a la versión vigente del presupuesto y mueve el estado de la
/// orden (a "aprobado" o "presupuesto_rechazado"). Solo se puede responder mientras la orden está
/// "esperando_aprobacion", y solo una vez por versión. Todo se valida antes de tocar nada, igual que
/// <see cref="CambiarEstado.CambiarEstadoOrdenHandler"/>; el cambio de estado se registra igual que ahí.
/// </summary>
public sealed class ResponderPresupuestoHandler(
    IOrdenReparacionRepositorio ordenes,
    IClienteRepositorio clientes,
    IDispositivoRepositorio dispositivos,
    IAuditoriaRepositorio auditoria,
    IDirectorioDeUsuarios directorio,
    ILocalActual local,
    ICurrentUserAccessor usuarioActual,
    TimeProvider reloj)
{
    public async Task<Resultado<OrdenReparacionDto>> HandleAsync(ResponderPresupuestoCommand comando, CancellationToken ct)
    {
        var orden = await ordenes.ObtenerPorIdAsync(comando.OrdenId, ct);
        if (orden is null)
        {
            return Resultado<OrdenReparacionDto>.Falla(TipoError.NoEncontrado, "orden_inexistente", "La orden no existe.");
        }

        if (orden.Estado != EstadosDeReparacion.EsperandoAprobacion)
        {
            return Resultado<OrdenReparacionDto>.Falla(
                TipoError.Conflicto,
                "fuera_de_esperando_aprobacion",
                "Solo se puede responder el presupuesto mientras la orden está esperando aprobación.");
        }

        var vigente = await ordenes.ObtenerVersionVigentePresupuestoAsync(orden.Id, ct);
        if (vigente is null)
        {
            return Resultado<OrdenReparacionDto>.Falla(
                TipoError.Conflicto, "sin_presupuesto_vigente", "La orden no tiene ningún presupuesto para responder.");
        }

        if (vigente.RespuestaAprobada is not null)
        {
            return Resultado<OrdenReparacionDto>.Falla(
                TipoError.Conflicto, "presupuesto_ya_respondido", "Esta versión del presupuesto ya tiene una respuesta registrada.");
        }

        var nuevoEstado = comando.Aprobado ? EstadosDeReparacion.Aprobado : EstadosDeReparacion.PresupuestoRechazado;
        var usuarioId = usuarioActual.GetCurrentUser()?.UsuarioId;
        var ahora = reloj.GetUtcNow();
        var estadoAnterior = orden.Estado;

        vigente.RegistrarRespuesta(comando.Aprobado, usuarioId, ahora);
        orden.CambiarEstado(nuevoEstado);

        auditoria.Agregar(RegistroAuditoria.Registrar(
            local.LocalId,
            EntidadesAuditadas.Reparacion,
            orden.Id,
            AccionesAuditadas.PresupuestoRespondido,
            usuarioId,
            ahora,
            [
                new CambioDeCampo("version_presupuesto", null, vigente.Numero.ToString(CultureInfo.InvariantCulture)),
                new CambioDeCampo("respuesta", null, comando.Aprobado ? "aprobado" : "rechazado"),
            ]));

        auditoria.Agregar(RegistroAuditoria.Registrar(
            local.LocalId,
            EntidadesAuditadas.Reparacion,
            orden.Id,
            AccionesAuditadas.EstadoCambiado,
            usuarioId,
            ahora,
            [new CambioDeCampo("estado", estadoAnterior, nuevoEstado)]));

        // La respuesta, el cambio de estado y sus registros de auditoría se guardan juntos.
        await ordenes.GuardarCambiosAsync(ct);

        var armador = new ArmadorDeOrdenes(ordenes, clientes, dispositivos, directorio);
        return Resultado<OrdenReparacionDto>.Ok((await armador.CompletasAsync([orden], ct))[0]);
    }
}
