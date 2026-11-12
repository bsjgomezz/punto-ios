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

namespace PuntoIOS.Application.Features.Reparaciones.CambiarEstado;

public sealed record CambiarEstadoOrdenCommand(Guid OrdenId, string? NuevoEstado);

/// <summary>
/// Pasa la orden a otro estado si la transición está permitida y deja registrado el cambio (anterior y nuevo).
/// A "aprobado" y "presupuesto_rechazado" no se pasa por acá: solo con
/// <see cref="ResponderPresupuesto.ResponderPresupuestoHandler"/>. A "entregado" tampoco: solo con
/// <see cref="EntregarOrden.EntregarOrdenHandler"/>, que registra el cobro y la garantía (y borra el desbloqueo).
/// </summary>
public sealed class CambiarEstadoOrdenHandler(
    IOrdenReparacionRepositorio ordenes,
    IClienteRepositorio clientes,
    IDispositivoRepositorio dispositivos,
    IAuditoriaRepositorio auditoria,
    IDirectorioDeUsuarios directorio,
    ILocalActual local,
    ICurrentUserAccessor usuarioActual,
    TimeProvider reloj)
{
    public async Task<Resultado<OrdenReparacionDto>> HandleAsync(CambiarEstadoOrdenCommand comando, CancellationToken ct)
    {
        var orden = await ordenes.ObtenerPorIdAsync(comando.OrdenId, ct);
        if (orden is null)
        {
            return Resultado<OrdenReparacionDto>.Falla(TipoError.NoEncontrado, "orden_inexistente", "La orden no existe.");
        }

        if (string.IsNullOrWhiteSpace(comando.NuevoEstado) || !EstadosDeReparacion.Catalogo.ContainsKey(comando.NuevoEstado))
        {
            return Resultado<OrdenReparacionDto>.Falla(TipoError.Validacion, "estado_invalido", "El estado no existe.");
        }

        // Aprobar o rechazar el presupuesto también registra la respuesta en la versión vigente: por eso solo se llega a
        // esos dos estados con POST /reparaciones/{id}/presupuestos/respuesta, nunca con este cambio de estado genérico.
        if (comando.NuevoEstado is EstadosDeReparacion.Aprobado or EstadosDeReparacion.PresupuestoRechazado)
        {
            return Resultado<OrdenReparacionDto>.Falla(
                TipoError.Conflicto,
                "usar_respuesta_presupuesto",
                "Para aprobar o rechazar el presupuesto hay que usar el endpoint de respuesta del presupuesto, no el cambio de estado genérico.");
        }

        // Entregar el equipo también registra el cobro y la garantía: por eso solo se llega a "entregado" con
        // POST /reparaciones/{id}/entrega, nunca con este cambio de estado genérico.
        if (comando.NuevoEstado == EstadosDeReparacion.Entregado)
        {
            return Resultado<OrdenReparacionDto>.Falla(
                TipoError.Conflicto,
                "usar_entrega",
                "Para entregar el equipo hay que usar el endpoint de entrega (registra cobro y garantía), no el cambio de estado genérico.");
        }

        if (orden.Estado == comando.NuevoEstado || !MaquinaDeEstados.PuedeTransicionar(orden.Estado, comando.NuevoEstado))
        {
            return Resultado<OrdenReparacionDto>.Falla(
                TipoError.Conflicto,
                "transicion_invalida",
                $"No se puede pasar de \"{orden.Estado}\" a \"{comando.NuevoEstado}\".");
        }

        var usuarioId = usuarioActual.GetCurrentUser()?.UsuarioId;
        var ahora = reloj.GetUtcNow();
        var estadoAnterior = orden.Estado;
        orden.CambiarEstado(comando.NuevoEstado);

        auditoria.Agregar(RegistroAuditoria.Registrar(
            local.LocalId,
            EntidadesAuditadas.Reparacion,
            orden.Id,
            AccionesAuditadas.EstadoCambiado,
            usuarioId,
            ahora,
            [new CambioDeCampo("estado", estadoAnterior, comando.NuevoEstado)]));

        // El cambio de estado y su registro de auditoría se guardan juntos.
        await ordenes.GuardarCambiosAsync(ct);

        var armador = new ArmadorDeOrdenes(ordenes, clientes, dispositivos, directorio);
        return Resultado<OrdenReparacionDto>.Ok((await armador.CompletasAsync([orden], ct))[0]);
    }
}
