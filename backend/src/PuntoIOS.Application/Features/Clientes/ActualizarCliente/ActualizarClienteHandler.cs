using PuntoIOS.Application.Common.Multilocal;
using PuntoIOS.Application.Common.Results;
using PuntoIOS.Application.Common.Security;
using PuntoIOS.Application.Features.Auditoria.Puertos;
using PuntoIOS.Application.Features.Clientes.Puertos;
using PuntoIOS.Domain.Auditoria;
using PuntoIOS.Domain.Clientes;

namespace PuntoIOS.Application.Features.Clientes.ActualizarCliente;

public sealed record ActualizarClienteCommand(
    Guid ClienteId,
    string? Nombre,
    string? Apellido,
    string? Telefono,
    string? Email,
    string? Direccion,
    string? Observaciones,
    bool PermitirDuplicado);

/// <summary>
/// Edita los datos de un cliente y deja constancia de qué campos cambiaron, quién y cuándo.
/// Si no cambió nada no se registra nada. Si cambia el teléfono se avisa de coincidencias con otros clientes.
/// </summary>
public sealed class ActualizarClienteHandler(
    IClienteRepositorio clientes,
    IAuditoriaRepositorio auditoria,
    ILocalActual local,
    ICurrentUserAccessor usuarioActual,
    TimeProvider reloj)
{
    public async Task<Resultado<ClienteDto>> HandleAsync(ActualizarClienteCommand comando, CancellationToken ct)
    {
        var cliente = await clientes.ObtenerPorIdAsync(comando.ClienteId, ct);
        if (cliente is null)
        {
            return Resultado<ClienteDto>.Falla(TipoError.NoEncontrado, "cliente_inexistente", "El cliente no existe.");
        }

        var error = ClienteValidacion.Validar(
            comando.Nombre, comando.Apellido, comando.Telefono, comando.Email, comando.Direccion, comando.Observaciones);
        if (error is not null)
        {
            return Resultado<ClienteDto>.Falla(error);
        }

        var telefonoNormalizado = Cliente.NormalizarTelefono(comando.Telefono);
        if (!comando.PermitirDuplicado && telefonoNormalizado != cliente.TelefonoNormalizado)
        {
            var coincidencias = (await clientes.BuscarPorTelefonoAsync(telefonoNormalizado, ct))
                .Where(c => c.Id != cliente.Id)
                .ToList();
            if (coincidencias.Count > 0)
            {
                return Resultado<ClienteDto>.Falla(ClienteValidacion.TelefonoDuplicado(coincidencias));
            }
        }

        var cambios = cliente.Actualizar(
            comando.Nombre!, comando.Apellido!, comando.Telefono!, comando.Email, comando.Direccion, comando.Observaciones);

        if (cambios.Count > 0)
        {
            auditoria.Agregar(RegistroAuditoria.Registrar(
                local.LocalId,
                EntidadesAuditadas.Cliente,
                cliente.Id,
                AccionesAuditadas.Editado,
                usuarioActual.GetCurrentUser()?.UsuarioId,
                reloj.GetUtcNow(),
                cambios));
            await clientes.GuardarCambiosAsync(ct);
        }

        return Resultado<ClienteDto>.Ok(ClienteDto.Desde(cliente));
    }
}
