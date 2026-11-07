using PuntoIOS.Application.Common.Multilocal;
using PuntoIOS.Application.Common.Results;
using PuntoIOS.Application.Common.Security;
using PuntoIOS.Application.Features.Auditoria.Puertos;
using PuntoIOS.Application.Features.Clientes.Puertos;
using PuntoIOS.Domain.Auditoria;
using PuntoIOS.Domain.Clientes;

namespace PuntoIOS.Application.Features.Clientes.CrearCliente;

public sealed record CrearClienteCommand(
    string? Nombre,
    string? Apellido,
    string? Telefono,
    string? Email,
    string? Direccion,
    string? Observaciones,
    bool PermitirDuplicado);

/// <summary>
/// Registra un cliente. Si ya hay uno con el mismo teléfono responde un conflicto con las coincidencias,
/// salvo que se confirme explícitamente (p. ej. familiares que comparten el número).
/// </summary>
public sealed class CrearClienteHandler(
    IClienteRepositorio clientes,
    IAuditoriaRepositorio auditoria,
    ILocalActual local,
    ICurrentUserAccessor usuarioActual,
    TimeProvider reloj)
{
    public async Task<Resultado<ClienteDto>> HandleAsync(CrearClienteCommand comando, CancellationToken ct)
    {
        var error = ClienteValidacion.Validar(
            comando.Nombre, comando.Apellido, comando.Telefono, comando.Email, comando.Direccion, comando.Observaciones);
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
        await clientes.GuardarCambiosAsync(ct);

        return Resultado<ClienteDto>.Ok(ClienteDto.Desde(cliente));
    }
}
