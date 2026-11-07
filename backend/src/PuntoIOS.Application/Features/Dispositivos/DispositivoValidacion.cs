using PuntoIOS.Application.Common.Results;
using PuntoIOS.Application.Features.Clientes;
using PuntoIOS.Domain.Clientes;

namespace PuntoIOS.Application.Features.Dispositivos;

/// <summary>Reglas de validación de un equipo, compartidas por el alta y la edición.</summary>
internal static class DispositivoValidacion
{
    public static Error? Validar(
        string? marca,
        string? modelo,
        string? almacenamiento,
        string? color,
        string? imeiSerie,
        string? observaciones)
    {
        if (string.IsNullOrWhiteSpace(marca))
        {
            return Invalido("marca_obligatoria", "La marca es obligatoria.");
        }

        if (string.IsNullOrWhiteSpace(modelo))
        {
            return Invalido("modelo_obligatorio", "El modelo es obligatorio.");
        }

        if (marca.Trim().Length > Dispositivo.LargoMaximoMarca
            || modelo.Trim().Length > Dispositivo.LargoMaximoModelo
            || (almacenamiento?.Trim().Length ?? 0) > Dispositivo.LargoMaximoAlmacenamiento
            || (color?.Trim().Length ?? 0) > Dispositivo.LargoMaximoColor
            || (observaciones?.Trim().Length ?? 0) > Dispositivo.LargoMaximoObservaciones)
        {
            return Invalido("texto_demasiado_largo", "Algún dato supera el largo permitido.");
        }

        var imei = Dispositivo.NormalizarImei(imeiSerie);
        if (imei is not null && !Dispositivo.ImeiEsValido(imei))
        {
            return Invalido(
                "imei_invalido",
                $"El IMEI debe tener 15 dígitos válidos, o el número de serie entre {Dispositivo.MinimoCaracteresImei} y {Dispositivo.MaximoCaracteresImei} letras o números.");
        }

        return null;
    }

    /// <summary>El IMEI ya pertenece a otro equipo: no se permite guardarlo (decisión del negocio).</summary>
    public static Error ImeiDuplicado(Cliente? duenio) =>
        new(
            TipoError.Conflicto,
            "imei_duplicado",
            "Ese IMEI o número de serie ya está registrado en otro equipo.",
            new Dictionary<string, object?>
            {
                ["cliente"] = duenio is null ? null : ClienteResumenDto.Desde(duenio),
            });

    private static Error Invalido(string codigo, string mensaje) => new(TipoError.Validacion, codigo, mensaje);
}
