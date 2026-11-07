using PuntoIOS.Application.Common.Multilocal;
using PuntoIOS.Application.Common.Results;
using PuntoIOS.Application.Common.Security;
using PuntoIOS.Application.Common.Validation;
using PuntoIOS.Application.Features.Clientes.Puertos;
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
    ILocalActual local,
    ICurrentUserAccessor usuarioActual,
    TimeProvider reloj)
{
    public async Task<Resultado<ClienteDto>> HandleAsync(CrearClienteCommand comando, CancellationToken ct)
    {
        var error = Validar(comando);
        if (error is not null)
        {
            return Resultado<ClienteDto>.Falla(error);
        }

        var telefonoNormalizado = Cliente.NormalizarTelefono(comando.Telefono);

        if (!comando.PermitirDuplicado)
        {
            var coincidencias = await clientes.BuscarPorTelefonoAsync(telefonoNormalizado, ct);
            if (coincidencias.Count > 0)
            {
                return Resultado<ClienteDto>.Falla(new Error(
                    TipoError.Conflicto,
                    "telefono_duplicado",
                    "Ya hay un cliente con ese teléfono.",
                    new Dictionary<string, object?>
                    {
                        ["coincidencias"] = coincidencias.Select(ClienteResumenDto.Desde).ToList(),
                    }));
            }
        }

        var cliente = Cliente.Registrar(
            local.LocalId,
            comando.Nombre!,
            comando.Apellido!,
            comando.Telefono!,
            comando.Email,
            comando.Direccion,
            comando.Observaciones,
            usuarioActual.GetCurrentUser()?.UsuarioId,
            reloj.GetUtcNow());

        clientes.Agregar(cliente);
        await clientes.GuardarCambiosAsync(ct);

        return Resultado<ClienteDto>.Ok(ClienteDto.Desde(cliente));
    }

    private static Error? Validar(CrearClienteCommand c)
    {
        if (string.IsNullOrWhiteSpace(c.Nombre))
        {
            return Invalido("nombre_obligatorio", "El nombre es obligatorio.");
        }

        if (string.IsNullOrWhiteSpace(c.Apellido))
        {
            return Invalido("apellido_obligatorio", "El apellido es obligatorio.");
        }

        if (string.IsNullOrWhiteSpace(c.Telefono))
        {
            return Invalido("telefono_obligatorio", "El teléfono es obligatorio.");
        }

        var digitos = Cliente.ContarDigitos(c.Telefono);
        if (digitos is < Cliente.MinimoDigitosTelefono or > Cliente.MaximoDigitosTelefono)
        {
            return Invalido(
                "telefono_invalido",
                $"El teléfono debe tener entre {Cliente.MinimoDigitosTelefono} y {Cliente.MaximoDigitosTelefono} dígitos.");
        }

        if (c.Nombre.Trim().Length > Cliente.LargoMaximoNombre || c.Apellido.Trim().Length > Cliente.LargoMaximoNombre)
        {
            return Invalido("texto_demasiado_largo", $"Nombre y apellido admiten hasta {Cliente.LargoMaximoNombre} caracteres.");
        }

        if (!string.IsNullOrWhiteSpace(c.Email)
            && !EmailValidator.EsValido(c.Email.Trim().ToLowerInvariant()))
        {
            return Invalido("email_invalido", "El email no es válido.");
        }

        if ((c.Direccion?.Trim().Length ?? 0) > Cliente.LargoMaximoDireccion)
        {
            return Invalido("texto_demasiado_largo", $"La dirección admite hasta {Cliente.LargoMaximoDireccion} caracteres.");
        }

        if ((c.Observaciones?.Trim().Length ?? 0) > Cliente.LargoMaximoObservaciones)
        {
            return Invalido("texto_demasiado_largo", $"Las observaciones admiten hasta {Cliente.LargoMaximoObservaciones} caracteres.");
        }

        return null;
    }

    private static Error Invalido(string codigo, string mensaje) => new(TipoError.Validacion, codigo, mensaje);
}
