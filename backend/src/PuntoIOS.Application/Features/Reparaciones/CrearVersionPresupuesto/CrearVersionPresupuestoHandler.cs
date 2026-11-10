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

namespace PuntoIOS.Application.Features.Reparaciones.CrearVersionPresupuesto;

public sealed record ItemPresupuestoInput(string? Descripcion, string? Tipo, decimal Monto);

public sealed record CrearVersionPresupuestoCommand(Guid OrdenId, List<ItemPresupuestoInput>? Items);

/// <summary>
/// Crea una versión nueva del presupuesto de la orden (nunca se edita una existente) y deja registro de la versión y
/// el total, anteriores y nuevos. Según el estado actual, la orden avanza:
/// esperando presupuesto → presupuesto enviado; presupuesto rechazado → esperando presupuesto → presupuesto enviado;
/// aprobado o en reparación → esperando aprobación. En otro estado no se toca. Cada cambio de estado se registra
/// igual que en <see cref="CambiarEstado.CambiarEstadoOrdenHandler"/>. Una orden entregada o cancelada no se presupuesta.
/// </summary>
public sealed class CrearVersionPresupuestoHandler(
    IOrdenReparacionRepositorio ordenes,
    IClienteRepositorio clientes,
    IDispositivoRepositorio dispositivos,
    IAuditoriaRepositorio auditoria,
    IDirectorioDeUsuarios directorio,
    ILocalActual local,
    ICurrentUserAccessor usuarioActual,
    TimeProvider reloj)
{
    public async Task<Resultado<OrdenReparacionDto>> HandleAsync(CrearVersionPresupuestoCommand comando, CancellationToken ct)
    {
        var orden = await ordenes.ObtenerPorIdAsync(comando.OrdenId, ct);
        if (orden is null)
        {
            return Resultado<OrdenReparacionDto>.Falla(TipoError.NoEncontrado, "orden_inexistente", "La orden no existe.");
        }

        if (orden.Estado is EstadosDeReparacion.Entregado or EstadosDeReparacion.Cancelado)
        {
            return Resultado<OrdenReparacionDto>.Falla(
                TipoError.Conflicto, "orden_finalizada", "No se puede presupuestar una orden entregada o cancelada.");
        }

        var items = (comando.Items ?? [])
            .Select(i => new ItemDePresupuesto(i?.Descripcion ?? string.Empty, i?.Tipo ?? string.Empty, i?.Monto ?? 0))
            .ToList();

        var anterior = await ordenes.ObtenerVersionVigentePresupuestoAsync(orden.Id, ct);
        var numero = (anterior?.Numero ?? 0) + 1;
        var ahora = reloj.GetUtcNow();
        var usuarioId = usuarioActual.GetCurrentUser()?.UsuarioId;

        VersionDePresupuesto version;
        try
        {
            // Las reglas (cantidad de ítems, descripción, tipo, monto) viven en el dominio. Se valida todo antes de
            // tocar la orden: si algo no es válido no se guarda la versión ni cambia el estado.
            version = VersionDePresupuesto.Crear(local.LocalId, orden.Id, numero, items, usuarioId, ahora);
        }
        catch (ArgumentException ex)
        {
            return Resultado<OrdenReparacionDto>.Falla(TipoError.Validacion, "presupuesto_invalido", MensajeSinParametro(ex));
        }

        ordenes.AgregarVersionPresupuesto(version);
        auditoria.Agregar(RegistroAuditoria.Registrar(
            local.LocalId,
            EntidadesAuditadas.Reparacion,
            orden.Id,
            AccionesAuditadas.PresupuestoCreado,
            usuarioId,
            ahora,
            [
                new CambioDeCampo("version_presupuesto", anterior?.Numero.ToString(CultureInfo.InvariantCulture), numero.ToString(CultureInfo.InvariantCulture)),
                new CambioDeCampo("total_presupuesto", anterior is null ? null : Monto(anterior.Total), Monto(version.Total)),
            ]));

        switch (orden.Estado)
        {
            case EstadosDeReparacion.EsperandoPresupuesto:
                Avanzar(orden, EstadosDeReparacion.PresupuestoEnviado, usuarioId, ahora);
                break;
            case EstadosDeReparacion.PresupuestoRechazado:
                // Dos transiciones válidas encadenadas, cada una con su registro.
                Avanzar(orden, EstadosDeReparacion.EsperandoPresupuesto, usuarioId, ahora);
                Avanzar(orden, EstadosDeReparacion.PresupuestoEnviado, usuarioId, ahora);
                break;
            case EstadosDeReparacion.Aprobado:
            case EstadosDeReparacion.EnReparacion:
                Avanzar(orden, EstadosDeReparacion.EsperandoAprobacion, usuarioId, ahora);
                break;
            default:
                // En cualquier otro estado (incluidos presupuesto enviado y esperando aprobación) no se toca.
                break;
        }

        // La versión, los cambios de estado y sus registros de auditoría se guardan juntos.
        await ordenes.GuardarCambiosAsync(ct);

        var armador = new ArmadorDeOrdenes(ordenes, clientes, dispositivos, directorio);
        return Resultado<OrdenReparacionDto>.Ok((await armador.CompletasAsync([orden], ct))[0]);
    }

    /// <summary>Cambia el estado y lo registra exactamente como <see cref="CambiarEstado.CambiarEstadoOrdenHandler"/>.</summary>
    private void Avanzar(OrdenReparacion orden, string nuevoEstado, Guid? usuarioId, DateTimeOffset ahora)
    {
        var estadoAnterior = orden.Estado;
        orden.CambiarEstado(nuevoEstado);
        auditoria.Agregar(RegistroAuditoria.Registrar(
            local.LocalId,
            EntidadesAuditadas.Reparacion,
            orden.Id,
            AccionesAuditadas.EstadoCambiado,
            usuarioId,
            ahora,
            [new CambioDeCampo("estado", estadoAnterior, nuevoEstado)]));
    }

    /// <summary>Monto para el historial, siempre con punto y dos decimales ("15000.50").</summary>
    private static string Monto(decimal valor) => valor.ToString("0.00", CultureInfo.InvariantCulture);

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
