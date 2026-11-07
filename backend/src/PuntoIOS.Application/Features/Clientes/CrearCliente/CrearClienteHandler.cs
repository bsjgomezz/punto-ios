using PuntoIOS.Application.Common.Multilocal;
using PuntoIOS.Application.Common.Results;
using PuntoIOS.Application.Common.Security;
using PuntoIOS.Application.Features.Auditoria.Puertos;
using PuntoIOS.Application.Features.Clientes.Puertos;
using PuntoIOS.Application.Features.Dispositivos;
using PuntoIOS.Application.Features.Dispositivos.Puertos;
using PuntoIOS.Domain.Auditoria;
using PuntoIOS.Domain.Clientes;

namespace PuntoIOS.Application.Features.Clientes.CrearCliente;

/// <summary>Equipo que trae el cliente al darlo de alta (opcional).</summary>
public sealed record EquipoNuevo(
    string? Marca,
    string? Modelo,
    string? Almacenamiento,
    string? Color,
    string? ImeiSerie);

public sealed record CrearClienteCommand(
    string? Nombre,
    string? Apellido,
    string? Telefono,
    string? Email,
    string? Direccion,
    string? Observaciones,
    bool PermitirDuplicado,
    EquipoNuevo? Equipo = null);

/// <summary>
/// Registra un cliente y, si viene, el equipo que trae, en una sola operación: o se guardan los dos o ninguno.
/// Un teléfono repetido responde conflicto con las coincidencias, salvo que se confirme (p. ej. familiares).
/// Un IMEI repetido no se permite.
/// </summary>
public sealed class CrearClienteHandler(
    IClienteRepositorio clientes,
    IDispositivoRepositorio dispositivos,
    IAuditoriaRepositorio auditoria,
    ILocalActual local,
    ICurrentUserAccessor usuarioActual,
    TimeProvider reloj)
{
    public async Task<Resultado<ClienteDto>> HandleAsync(CrearClienteCommand comando, CancellationToken ct)
    {
        var error = ClienteValidacion.Validar(
            comando.Nombre, comando.Apellido, comando.Telefono, comando.Email, comando.Direccion, comando.Observaciones);
        if (error is null && comando.Equipo is { } e)
        {
            error = DispositivoValidacion.Validar(e.Marca, e.Modelo, e.Almacenamiento, e.Color, e.ImeiSerie, null);
        }

        if (error is not null)
        {
            return Resultado<ClienteDto>.Falla(error);
        }

        if (!comando.PermitirDuplicado)
        {
            var coincidencias = await clientes.BuscarPorTelefonoAsync(Cliente.NormalizarTelefono(comando.Telefono), ct);
            if (coincidencias.Count > 0)
            {
                return Resultado<ClienteDto>.Falla(ClienteValidacion.TelefonoDuplicado(coincidencias));
            }
        }

        var imei = Dispositivo.NormalizarImei(comando.Equipo?.ImeiSerie);
        if (imei is not null && await dispositivos.ObtenerPorImeiAsync(imei, ct) is { } existente)
        {
            return Resultado<ClienteDto>.Falla(
                DispositivoValidacion.ImeiDuplicado(await clientes.ObtenerPorIdAsync(existente.ClienteId, ct)));
        }

        var usuarioId = usuarioActual.GetCurrentUser()?.UsuarioId;
        var ahora = reloj.GetUtcNow();

        var cliente = Cliente.Registrar(
            local.LocalId,
            comando.Nombre!,
            comando.Apellido!,
            comando.Telefono!,
            comando.Email,
            comando.Direccion,
            comando.Observaciones,
            usuarioId,
            ahora);

        clientes.Agregar(cliente);
        auditoria.Agregar(RegistroAuditoria.Registrar(
            local.LocalId, EntidadesAuditadas.Cliente, cliente.Id, AccionesAuditadas.Creado, usuarioId, ahora));

        if (comando.Equipo is { } equipo)
        {
            var dispositivo = Dispositivo.Registrar(
                local.LocalId,
                cliente.Id,
                equipo.Marca!,
                equipo.Modelo!,
                equipo.Almacenamiento,
                equipo.Color,
                equipo.ImeiSerie,
                null,
                usuarioId,
                ahora);
            dispositivos.Agregar(dispositivo);
            auditoria.Agregar(RegistroAuditoria.Registrar(
                local.LocalId, EntidadesAuditadas.Dispositivo, dispositivo.Id, AccionesAuditadas.Creado, usuarioId, ahora));
        }

        // Clientes, equipos y auditoría comparten la misma unidad de trabajo: un único guardado.
        await clientes.GuardarCambiosAsync(ct);

        return Resultado<ClienteDto>.Ok(ClienteDto.Desde(cliente));
    }
}
