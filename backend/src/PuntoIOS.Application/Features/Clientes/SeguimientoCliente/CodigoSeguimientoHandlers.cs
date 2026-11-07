using PuntoIOS.Application.Common.Multilocal;
using PuntoIOS.Application.Common.Results;
using PuntoIOS.Application.Common.Security;
using PuntoIOS.Application.Features.Auditoria.Puertos;
using PuntoIOS.Application.Features.Clientes.Puertos;
using PuntoIOS.Domain.Auditoria;
using PuntoIOS.Domain.Clientes;

namespace PuntoIOS.Application.Features.Clientes.SeguimientoCliente;

/// <summary>Código de seguimiento listo para mostrar (<c>Codigo</c> sin guiones, <c>Formateado</c> en grupos de 4).</summary>
public sealed record CodigoSeguimientoDto(string Codigo, string Formateado, DateTimeOffset GeneradoEn);

/// <summary>
/// Genera el código de seguimiento del cliente o lo regenera: el anterior deja de funcionar al instante.
/// </summary>
public sealed class GenerarCodigoSeguimientoHandler(
    IClienteRepositorio clientes,
    IAuditoriaRepositorio auditoria,
    ICifradoDeDatos cifrado,
    ILocalActual local,
    ICurrentUserAccessor usuarioActual,
    TimeProvider reloj)
{
    public async Task<Resultado<CodigoSeguimientoDto>> HandleAsync(Guid clienteId, CancellationToken ct)
    {
        var cliente = await clientes.ObtenerPorIdAsync(clienteId, ct);
        if (cliente is null)
        {
            return Resultado<CodigoSeguimientoDto>.Falla(TipoError.NoEncontrado, "cliente_inexistente", "El cliente no existe.");
        }

        var codigo = CodigoSeguimiento.Generar();
        var ahora = reloj.GetUtcNow();
        var regenerado = cliente.AsignarCodigoSeguimiento(
            CodigoSeguimiento.Huella(codigo), cifrado.Cifrar(codigo), ahora);

        auditoria.Agregar(RegistroAuditoria.Registrar(
            local.LocalId,
            EntidadesAuditadas.Cliente,
            cliente.Id,
            regenerado ? AccionesAuditadas.CodigoRegenerado : AccionesAuditadas.CodigoGenerado,
            usuarioActual.GetCurrentUser()?.UsuarioId,
            ahora));
        await clientes.GuardarCambiosAsync(ct);

        return Resultado<CodigoSeguimientoDto>.Ok(
            new CodigoSeguimientoDto(codigo, CodigoSeguimiento.Formatear(codigo), ahora));
    }
}

/// <summary>Devuelve el código vigente para mostrarlo o enviarlo, y deja registrado quién lo consultó.</summary>
public sealed class ConsultarCodigoSeguimientoHandler(
    IClienteRepositorio clientes,
    IAuditoriaRepositorio auditoria,
    ICifradoDeDatos cifrado,
    ILocalActual local,
    ICurrentUserAccessor usuarioActual,
    TimeProvider reloj)
{
    public async Task<Resultado<CodigoSeguimientoDto>> HandleAsync(Guid clienteId, CancellationToken ct)
    {
        var cliente = await clientes.ObtenerPorIdAsync(clienteId, ct);
        if (cliente is null)
        {
            return Resultado<CodigoSeguimientoDto>.Falla(TipoError.NoEncontrado, "cliente_inexistente", "El cliente no existe.");
        }

        if (cliente.CodigoSeguimientoCifrado is null || cliente.CodigoSeguimientoGeneradoEn is null)
        {
            return Resultado<CodigoSeguimientoDto>.Falla(
                TipoError.NoEncontrado, "codigo_inexistente", "El cliente todavía no tiene código de seguimiento.");
        }

        var codigo = cifrado.Descifrar(cliente.CodigoSeguimientoCifrado);

        auditoria.Agregar(RegistroAuditoria.Registrar(
            local.LocalId,
            EntidadesAuditadas.Cliente,
            cliente.Id,
            AccionesAuditadas.CodigoConsultado,
            usuarioActual.GetCurrentUser()?.UsuarioId,
            reloj.GetUtcNow()));
        await clientes.GuardarCambiosAsync(ct);

        return Resultado<CodigoSeguimientoDto>.Ok(new CodigoSeguimientoDto(
            codigo, CodigoSeguimiento.Formatear(codigo), cliente.CodigoSeguimientoGeneradoEn.Value));
    }
}
