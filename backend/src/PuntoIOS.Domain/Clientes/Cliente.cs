namespace PuntoIOS.Domain.Clientes;

/// <summary>Cliente del negocio. Nombre, apellido y teléfono son obligatorios; el resto es opcional.</summary>
public sealed class Cliente
{
    public const int LargoMaximoNombre = 100;
    public const int LargoMaximoEmail = 254;
    public const int LargoMaximoDireccion = 200;
    public const int LargoMaximoObservaciones = 1000;
    public const int MinimoDigitosTelefono = 6;
    public const int MaximoDigitosTelefono = 15;

    /// <summary>Cantidad de dígitos finales que se comparan para detectar teléfonos repetidos.</summary>
    private const int DigitosComparables = 10;

    private Cliente()
    {
    }

    public Guid Id { get; private set; }

    public Guid LocalId { get; private set; }

    public string Nombre { get; private set; } = string.Empty;

    public string Apellido { get; private set; } = string.Empty;

    /// <summary>Teléfono tal como lo escribió quien cargó el cliente.</summary>
    public string Telefono { get; private set; } = string.Empty;

    /// <summary>Últimos dígitos del teléfono, sin símbolos ni prefijos: sirve para buscar y detectar duplicados.</summary>
    public string TelefonoNormalizado { get; private set; } = string.Empty;

    public string? Email { get; private set; }

    public string? Direccion { get; private set; }

    public string? Observaciones { get; private set; }

    public Guid? CreadoPorUsuarioId { get; private set; }

    public DateTimeOffset CreadoEn { get; private set; }

    /// <summary>Deja solo los dígitos y conserva los últimos 10, para que "+54 9 3498 12-3456" y "3498123456" coincidan.</summary>
    public static string NormalizarTelefono(string? telefono)
    {
        var digitos = new string((telefono ?? string.Empty).Where(char.IsAsciiDigit).ToArray());
        return digitos.Length > DigitosComparables ? digitos[^DigitosComparables..] : digitos;
    }

    public static int ContarDigitos(string? telefono) => (telefono ?? string.Empty).Count(char.IsAsciiDigit);

    public static Cliente Registrar(
        Guid localId,
        string nombre,
        string apellido,
        string telefono,
        string? email,
        string? direccion,
        string? observaciones,
        Guid? creadoPorUsuarioId,
        DateTimeOffset ahora)
    {
        if (string.IsNullOrWhiteSpace(nombre))
        {
            throw new ArgumentException("El nombre es obligatorio.", nameof(nombre));
        }

        if (string.IsNullOrWhiteSpace(apellido))
        {
            throw new ArgumentException("El apellido es obligatorio.", nameof(apellido));
        }

        var normalizado = NormalizarTelefono(telefono);
        if (ContarDigitos(telefono) < MinimoDigitosTelefono)
        {
            throw new ArgumentException("El teléfono no es válido.", nameof(telefono));
        }

        return new Cliente
        {
            Id = Guid.CreateVersion7(),
            LocalId = localId,
            Nombre = nombre.Trim(),
            Apellido = apellido.Trim(),
            Telefono = telefono.Trim(),
            TelefonoNormalizado = normalizado,
            Email = VacioANulo(email)?.ToLowerInvariant(),
            Direccion = VacioANulo(direccion),
            Observaciones = VacioANulo(observaciones),
            CreadoPorUsuarioId = creadoPorUsuarioId,
            CreadoEn = ahora,
        };
    }

    private static string? VacioANulo(string? valor) => string.IsNullOrWhiteSpace(valor) ? null : valor.Trim();
}
