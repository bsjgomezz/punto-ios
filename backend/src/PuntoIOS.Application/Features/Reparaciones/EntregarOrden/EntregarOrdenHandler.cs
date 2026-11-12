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

namespace PuntoIOS.Application.Features.Reparaciones.EntregarOrden;

public sealed record EntregarOrdenCommand(Guid OrdenId, string? GarantiaCodigo, decimal MontoCobrado, string? MetodoPago);

/// <summary>
/// Entrega el equipo: registra la garantía elegida y el cobro, y mueve el estado a "entregado". Solo desde
/// "listo_para_retirar", "presupuesto_rechazado" o "irreparable" — las únicas tres que la máquina de estados
/// deja llegar a "entregado". El cambio de estado genérico rechaza llegar directo a "entregado" (igual razón que
/// rechaza "aprobado"/"presupuesto_rechazado" desde S14): así nunca queda una entrega sin su cobro y garantía.
/// </summary>
public sealed class EntregarOrdenHandler(
    IOrdenReparacionRepositorio ordenes,
    IClienteRepositorio clientes,
    IDispositivoRepositorio dispositivos,
    IAuditoriaRepositorio auditoria,
    IDirectorioDeUsuarios directorio,
    ILocalActual local,
    ICurrentUserAccessor usuarioActual,
    TimeProvider reloj)
{
    public async Task<Resultado<OrdenReparacionDto>> HandleAsync(EntregarOrdenCommand comando, CancellationToken ct)
    {
        var orden = await ordenes.ObtenerPorIdAsync(comando.OrdenId, ct);
        if (orden is null)
        {
            return Resultado<OrdenReparacionDto>.Falla(TipoError.NoEncontrado, "orden_inexistente", "La orden no existe.");
        }

        if (!MaquinaDeEstados.PuedeTransicionar(orden.Estado, EstadosDeReparacion.Entregado))
        {
            return Resultado<OrdenReparacionDto>.Falla(
                TipoError.Conflicto,
                "fuera_de_estado_entregable",
                $"No se puede entregar una orden en estado \"{orden.Estado}\".");
        }

        var usuarioId = usuarioActual.GetCurrentUser()?.UsuarioId;
        var ahora = reloj.GetUtcNow();
        var estadoAnterior = orden.Estado;

        try
        {
            orden.RegistrarEntrega(comando.GarantiaCodigo, comando.MontoCobrado, comando.MetodoPago, ahora);
        }
        catch (ArgumentException ex)
        {
            return Resultado<OrdenReparacionDto>.Falla(TipoError.Validacion, "entrega_invalida", MensajeSinParametro(ex));
        }

        orden.CambiarEstado(EstadosDeReparacion.Entregado);

        auditoria.Agregar(RegistroAuditoria.Registrar(
            local.LocalId,
            EntidadesAuditadas.Reparacion,
            orden.Id,
            AccionesAuditadas.EntregaRegistrada,
            usuarioId,
            ahora,
            [
                new CambioDeCampo("garantia", null, OpcionesDeGarantia.Catalogo[orden.GarantiaCodigo!]),
                new CambioDeCampo("monto_cobrado", null, orden.MontoCobrado!.Value.ToString(CultureInfo.InvariantCulture)),
                new CambioDeCampo("metodo_pago", null, MetodosDePago.Catalogo[orden.MetodoPago!]),
            ]));
        auditoria.Agregar(RegistroAuditoria.Registrar(
            local.LocalId,
            EntidadesAuditadas.Reparacion,
            orden.Id,
            AccionesAuditadas.EstadoCambiado,
            usuarioId,
            ahora,
            [new CambioDeCampo("estado", estadoAnterior, EstadosDeReparacion.Entregado)]));

        // La entrega, el cambio de estado y su auditoría se guardan juntos.
        await ordenes.GuardarCambiosAsync(ct);

        var armador = new ArmadorDeOrdenes(ordenes, clientes, dispositivos, directorio);
        return Resultado<OrdenReparacionDto>.Ok((await armador.CompletasAsync([orden], ct))[0]);
    }

    /// <summary>
    /// <see cref="ArgumentException.Message"/> le agrega " (Parameter 'x')" al mensaje: se lo saca para mostrarlo
    /// a quien usa la app. Si el runtime lo arma distinto, se deja el mensaje completo.
    /// </summary>
    private static string MensajeSinParametro(ArgumentException ex)
    {
        var sufijo = $" (Parameter '{ex.ParamName}')";
        return ex.ParamName is not null && ex.Message.EndsWith(sufijo, StringComparison.Ordinal)
            ? ex.Message[..^sufijo.Length]
            : ex.Message;
    }
}
