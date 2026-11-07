using PuntoIOS.Domain.Auditoria;

namespace PuntoIOS.Domain.Clientes;

/// <summary>
/// Equipo de un cliente. El IMEI o número de serie es un dato sensible: se guarda normalizado
/// (solo letras y dígitos, en mayúsculas) y fuera de la ficha completa se muestra enmascarado.
/// </summary>
public sealed class Dispositivo
{
    public const int LargoMaximoMarca = 50;
    public const int LargoMaximoModelo = 100;
    public const int LargoMaximoAlmacenamiento = 30;
    public const int LargoMaximoColor = 50;
    public const int LargoMaximoObservaciones = 1000;
    public const int MinimoCaracteresImei = 8;
    public const int MaximoCaracteresImei = 20;

    /// <summary>Caracteres finales del IMEI que quedan visibles al enmascararlo.</summary>
    public const int CaracteresVisibles = 4;

    private const int LargoImei = 15;

    private Dispositivo()
    {
    }

    public Guid Id { get; private set; }

    public Guid LocalId { get; private set; }

    public Guid ClienteId { get; private set; }

    public string Marca { get; private set; } = string.Empty;

    public string Modelo { get; private set; } = string.Empty;

    public string? Almacenamiento { get; private set; }

    public string? Color { get; private set; }

    /// <summary>IMEI o número de serie normalizado. Nunca se devuelve completo salvo con permiso específico.</summary>
    public string? ImeiSerie { get; private set; }

    public string? Observaciones { get; private set; }

    public Guid? CreadoPorUsuarioId { get; private set; }

    public DateTimeOffset CreadoEn { get; private set; }

    public string? ImeiSerieEnmascarado => ImeiSerie is null ? null : Enmascarar(ImeiSerie);

    /// <summary>Deja solo letras y dígitos en mayúsculas ("35 123456 789012 3" → "351234567890123"). Vacío → null.</summary>
    public static string? NormalizarImei(string? valor)
    {
        var limpio = new string((valor ?? string.Empty).Where(char.IsAsciiLetterOrDigit).ToArray()).ToUpperInvariant();
        return limpio.Length == 0 ? null : limpio;
    }

    /// <summary>
    /// Un IMEI (15 dígitos) debe pasar el dígito verificador de Luhn. Un número de serie (letras y dígitos)
    /// solo se controla por largo.
    /// </summary>
    public static bool ImeiEsValido(string normalizado)
    {
        if (normalizado.Length is < MinimoCaracteresImei or > MaximoCaracteresImei)
        {
            return false;
        }

        return !(normalizado.Length == LargoImei && normalizado.All(char.IsAsciiDigit)) || CumpleLuhn(normalizado);
    }

    public static string Enmascarar(string normalizado) =>
        normalizado.Length <= CaracteresVisibles
            ? new string('•', normalizado.Length)
            : new string('•', normalizado.Length - CaracteresVisibles) + normalizado[^CaracteresVisibles..];

    public static Dispositivo Registrar(
        Guid localId,
        Guid clienteId,
        string marca,
        string modelo,
        string? almacenamiento,
        string? color,
        string? imeiSerie,
        string? observaciones,
        Guid? creadoPorUsuarioId,
        DateTimeOffset ahora)
    {
        ValidarObligatorios(marca, modelo);
        var imei = NormalizarImei(imeiSerie);
        if (imei is not null && !ImeiEsValido(imei))
        {
            throw new ArgumentException("El IMEI o número de serie no es válido.", nameof(imeiSerie));
        }

        return new Dispositivo
        {
            Id = Guid.CreateVersion7(),
            LocalId = localId,
            ClienteId = clienteId,
            Marca = marca.Trim(),
            Modelo = modelo.Trim(),
            Almacenamiento = VacioANulo(almacenamiento),
            Color = VacioANulo(color),
            ImeiSerie = imei,
            Observaciones = VacioANulo(observaciones),
            CreadoPorUsuarioId = creadoPorUsuarioId,
            CreadoEn = ahora,
        };
    }

    /// <summary>
    /// Reemplaza los datos editables y devuelve los campos que cambiaron. El IMEI solo cambia si
    /// <paramref name="actualizarImei"/> es verdadero, y en el historial queda enmascarado.
    /// </summary>
    public IReadOnlyList<CambioDeCampo> Actualizar(
        string marca,
        string modelo,
        string? almacenamiento,
        string? color,
        string? observaciones,
        bool actualizarImei,
        string? imeiSerie)
    {
        ValidarObligatorios(marca, modelo);
        var imeiNuevo = actualizarImei ? NormalizarImei(imeiSerie) : ImeiSerie;
        if (imeiNuevo is not null && imeiNuevo != ImeiSerie && !ImeiEsValido(imeiNuevo))
        {
            throw new ArgumentException("El IMEI o número de serie no es válido.", nameof(imeiSerie));
        }

        var cambios = new List<CambioDeCampo>();

        void Aplicar(string campo, string? anterior, string? nuevo, Action<string?> asignar)
        {
            if (anterior == nuevo)
            {
                return;
            }

            cambios.Add(new CambioDeCampo(campo, anterior, nuevo));
            asignar(nuevo);
        }

        Aplicar("marca", Marca, marca.Trim(), v => Marca = v!);
        Aplicar("modelo", Modelo, modelo.Trim(), v => Modelo = v!);
        Aplicar("almacenamiento", Almacenamiento, VacioANulo(almacenamiento), v => Almacenamiento = v);
        Aplicar("color", Color, VacioANulo(color), v => Color = v);

        if (imeiNuevo != ImeiSerie)
        {
            // En el historial nunca queda el IMEI completo.
            cambios.Add(new CambioDeCampo(
                "imei_serie",
                ImeiSerie is null ? null : Enmascarar(ImeiSerie),
                imeiNuevo is null ? null : Enmascarar(imeiNuevo)));
            ImeiSerie = imeiNuevo;
        }

        Aplicar("observaciones", Observaciones, VacioANulo(observaciones), v => Observaciones = v);

        return cambios;
    }

    private static void ValidarObligatorios(string marca, string modelo)
    {
        if (string.IsNullOrWhiteSpace(marca))
        {
            throw new ArgumentException("La marca es obligatoria.", nameof(marca));
        }

        if (string.IsNullOrWhiteSpace(modelo))
        {
            throw new ArgumentException("El modelo es obligatorio.", nameof(modelo));
        }
    }

    private static bool CumpleLuhn(string digitos)
    {
        var suma = 0;
        for (var i = 0; i < digitos.Length; i++)
        {
            var d = digitos[digitos.Length - 1 - i] - '0';
            if (i % 2 == 1)
            {
                d *= 2;
                if (d > 9)
                {
                    d -= 9;
                }
            }

            suma += d;
        }

        return suma % 10 == 0;
    }

    private static string? VacioANulo(string? valor) => string.IsNullOrWhiteSpace(valor) ? null : valor.Trim();
}
