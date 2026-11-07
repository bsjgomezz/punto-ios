using PuntoIOS.Application.Common.Results;
using PuntoIOS.Application.Common.Validation;
using PuntoIOS.Domain.Clientes;

namespace PuntoIOS.Application.Features.Clientes;

/// <summary>Reglas de validación de los datos de un cliente, compartidas por el alta y la edición.</summary>
internal static class ClienteValidacion
{
    public static Error? Validar(
        string? nombre,
        string? apellido,
        string? telefono,
        string? email,
        string? direccion,
        string? observaciones)
    {
        if (string.IsNullOrWhiteSpace(nombre))
        {
            return Invalido("nombre_obligatorio", "El nombre es obligatorio.");
        }

        if (string.IsNullOrWhiteSpace(apellido))
        {
            return Invalido("apellido_obligatorio", "El apellido es obligatorio.");
        }

        if (string.IsNullOrWhiteSpace(telefono))
        {
            return Invalido("telefono_obligatorio", "El teléfono es obligatorio.");
        }

        var digitos = Cliente.ContarDigitos(telefono);
        if (digitos is < Cliente.MinimoDigitosTelefono or > Cliente.MaximoDigitosTelefono)
        {
            return Invalido(
                "telefono_invalido",
                $"El teléfono debe tener entre {Cliente.MinimoDigitosTelefono} y {Cliente.MaximoDigitosTelefono} dígitos.");
        }

        if (nombre.Trim().Length > Cliente.LargoMaximoNombre || apellido.Trim().Length > Cliente.LargoMaximoNombre)
        {
            return Invalido("texto_demasiado_largo", $"Nombre y apellido admiten hasta {Cliente.LargoMaximoNombre} caracteres.");
        }

        if (!string.IsNullOrWhiteSpace(email) && !EmailValidator.EsValido(email.Trim().ToLowerInvariant()))
        {
            return Invalido("email_invalido", "El email no es válido.");
        }

        if ((direccion?.Trim().Length ?? 0) > Cliente.LargoMaximoDireccion)
        {
            return Invalido("texto_demasiado_largo", $"La dirección admite hasta {Cliente.LargoMaximoDireccion} caracteres.");
        }

        if ((observaciones?.Trim().Length ?? 0) > Cliente.LargoMaximoObservaciones)
        {
            return Invalido("texto_demasiado_largo", $"Las observaciones admiten hasta {Cliente.LargoMaximoObservaciones} caracteres.");
        }

        return null;
    }

    public static Error TelefonoDuplicado(IEnumerable<Cliente> coincidencias) =>
        new(
            TipoError.Conflicto,
            "telefono_duplicado",
            "Ya hay un cliente con ese teléfono.",
            new Dictionary<string, object?>
            {
                ["coincidencias"] = coincidencias.Select(ClienteResumenDto.Desde).ToList(),
            });

    private static Error Invalido(string codigo, string mensaje) => new(TipoError.Validacion, codigo, mensaje);
}
