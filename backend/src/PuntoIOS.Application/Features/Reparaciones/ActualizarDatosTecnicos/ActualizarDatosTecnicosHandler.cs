using PuntoIOS.Application.Common.Multilocal;
using PuntoIOS.Application.Common.Results;
using PuntoIOS.Application.Common.Security;
using PuntoIOS.Application.Features.Auditoria.Puertos;
using PuntoIOS.Application.Features.Clientes.Puertos;
using PuntoIOS.Application.Features.Dispositivos.Puertos;
using PuntoIOS.Application.Features.Identidad.Puertos;
using PuntoIOS.Application.Features.Reparaciones.Puertos;
using PuntoIOS.Domain.Auditoria;

namespace PuntoIOS.Application.Features.Reparaciones.ActualizarDatosTecnicos;

public sealed record ActualizarDatosTecnicosCommand(
    Guid OrdenId,
    string? Pruebas,
    string? Diagnostico,
    string? Mediciones,
    List<string>? ComponentesAfectados,
    string? NotasInternas);

/// <summary>
/// Reemplaza los datos técnicos de la orden (pruebas, diagnóstico, mediciones, componentes afectados y notas internas)
/// y deja constancia de qué campos cambiaron, quién y cuándo. Si no cambió nada no se registra nada.
/// </summary>
public sealed class ActualizarDatosTecnicosHandler(
    IOrdenReparacionRepositorio ordenes,
    IClienteRepositorio clientes,
    IDispositivoRepositorio dispositivos,
    IAuditoriaRepositorio auditoria,
    IDirectorioDeUsuarios directorio,
    ILocalActual local,
    ICurrentUserAccessor usuarioActual,
    TimeProvider reloj)
{
    public async Task<Resultado<OrdenReparacionDto>> HandleAsync(ActualizarDatosTecnicosCommand comando, CancellationToken ct)
    {
        var orden = await ordenes.ObtenerPorIdAsync(comando.OrdenId, ct);
        if (orden is null)
        {
            return Resultado<OrdenReparacionDto>.Falla(TipoError.NoEncontrado, "orden_inexistente", "La orden no existe.");
        }

        IReadOnlyList<CambioDeCampo> cambios;
        try
        {
            // Las reglas (largos, cantidad de componentes) viven en el dominio, que valida todo antes de cambiar
            // algún campo: si tira, la orden queda como estaba.
            cambios = orden.ActualizarDatosTecnicos(
                comando.Pruebas, comando.Diagnostico, comando.Mediciones, comando.ComponentesAfectados, comando.NotasInternas);
        }
        catch (ArgumentException ex)
        {
            return Resultado<OrdenReparacionDto>.Falla(TipoError.Validacion, "datos_tecnicos_invalidos", MensajeSinParametro(ex));
        }

        if (cambios.Count > 0)
        {
            auditoria.Agregar(RegistroAuditoria.Registrar(
                local.LocalId,
                EntidadesAuditadas.Reparacion,
                orden.Id,
                AccionesAuditadas.Editado,
                usuarioActual.GetCurrentUser()?.UsuarioId,
                reloj.GetUtcNow(),
                cambios));

            // Los datos técnicos y su registro de auditoría se guardan juntos.
            await ordenes.GuardarCambiosAsync(ct);
        }

        var armador = new ArmadorDeOrdenes(clientes, dispositivos, directorio);
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
