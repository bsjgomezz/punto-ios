using PuntoIOS.Application.Common.Multilocal;
using PuntoIOS.Application.Common.Results;
using PuntoIOS.Application.Common.Security;
using PuntoIOS.Application.Features.Auditoria.Puertos;
using PuntoIOS.Application.Features.Clientes.Puertos;
using PuntoIOS.Application.Features.Dispositivos.Puertos;
using PuntoIOS.Domain.Auditoria;
using PuntoIOS.Domain.Clientes;

namespace PuntoIOS.Application.Features.Dispositivos.CrearDispositivo;

public sealed record CrearDispositivoCommand(
    Guid ClienteId,
    string? Marca,
    string? Modelo,
    string? Almacenamiento,
    string? Color,
    string? ImeiSerie,
    string? Observaciones);

/// <summary>Registra un equipo de un cliente. Un IMEI ya registrado en otro equipo no se permite.</summary>
public sealed class CrearDispositivoHandler(
    IClienteRepositorio clientes,
    IDispositivoRepositorio dispositivos,
    IAuditoriaRepositorio auditoria,
    ILocalActual local,
    ICurrentUserAccessor usuarioActual,
    TimeProvider reloj)
{
    public async Task<Resultado<DispositivoDto>> HandleAsync(CrearDispositivoCommand comando, CancellationToken ct)
    {
        if (await clientes.ObtenerPorIdAsync(comando.ClienteId, ct) is null)
        {
            return Resultado<DispositivoDto>.Falla(TipoError.NoEncontrado, "cliente_inexistente", "El cliente no existe.");
        }

        var error = DispositivoValidacion.Validar(
            comando.Marca, comando.Modelo, comando.Almacenamiento, comando.Color, comando.ImeiSerie, comando.Observaciones);
        if (error is not null)
        {
            return Resultado<DispositivoDto>.Falla(error);
        }

        var imei = Dispositivo.NormalizarImei(comando.ImeiSerie);
        if (imei is not null && await dispositivos.ObtenerPorImeiAsync(imei, ct) is { } existente)
        {
            return Resultado<DispositivoDto>.Falla(
                DispositivoValidacion.ImeiDuplicado(await clientes.ObtenerPorIdAsync(existente.ClienteId, ct)));
        }

        var usuarioId = usuarioActual.GetCurrentUser()?.UsuarioId;
        var ahora = reloj.GetUtcNow();
        var dispositivo = Dispositivo.Registrar(
            local.LocalId,
            comando.ClienteId,
            comando.Marca!,
            comando.Modelo!,
            comando.Almacenamiento,
            comando.Color,
            comando.ImeiSerie,
            comando.Observaciones,
            usuarioId,
            ahora);

        dispositivos.Agregar(dispositivo);
        auditoria.Agregar(RegistroAuditoria.Registrar(
            local.LocalId, EntidadesAuditadas.Dispositivo, dispositivo.Id, AccionesAuditadas.Creado, usuarioId, ahora));
        await dispositivos.GuardarCambiosAsync(ct);

        return Resultado<DispositivoDto>.Ok(DispositivoDto.Desde(dispositivo));
    }
}
