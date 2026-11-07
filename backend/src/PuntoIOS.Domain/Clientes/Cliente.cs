using PuntoIOS.Domain.Auditoria;

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

    /// <summary>Huella del código de seguimiento vigente (null si nunca se generó).</summary>
    public string? CodigoSeguimientoHuella { get; private set; }

    /// <summary>Código de seguimiento cifrado, para poder volver a mostrarlo al personal autorizado.</summary>
    public string? CodigoSeguimientoCifrado { get; private set; }

    public DateTimeOffset? CodigoSeguimientoGeneradoEn { get; private set; }

    /// <summary>
    /// Asigna un código nuevo. Si ya tenía uno, el anterior deja de funcionar. Devuelve si fue una regeneración.
    /// </summary>
    public bool AsignarCodigoSeguimiento(string huella, string cifrado, DateTimeOffset ahora)
    {
        if (string.IsNullOrWhiteSpace(huella) || string.IsNullOrWhiteSpace(cifrado))
        {
            throw new ArgumentException("El código de seguimiento es obligatorio.");
        }

        var regenerado = CodigoSeguimientoHuella is not null;
        CodigoSeguimientoHuella = huella;
        CodigoSeguimientoCifrado = cifrado;
        CodigoSeguimientoGeneradoEn = ahora;
        return regenerado;
    }

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

    /// <summary>Reemplaza los datos editables y devuelve únicamente los campos que realmente cambiaron.</summary>
    public IReadOnlyList<CambioDeCampo> Actualizar(
        string nombre,
        string apellido,
        string telefono,
        string? email,
        string? direccion,
        string? observaciones)
    {
        if (string.IsNullOrWhiteSpace(nombre))
        {
            throw new ArgumentException("El nombre es obligatorio.", nameof(nombre));
        }

        if (string.IsNullOrWhiteSpace(apellido))
        {
            throw new ArgumentException("El apellido es obligatorio.", nameof(apellido));
        }

        if (ContarDigitos(telefono) < MinimoDigitosTelefono)
        {
            throw new ArgumentException("El teléfono no es válido.", nameof(telefono));
        }

        var cambios = new List<CambioDeCampo>();

        var nombreNuevo = nombre.Trim();
        if (nombreNuevo != Nombre)
        {
            cambios.Add(new CambioDeCampo("nombre", Nombre, nombreNuevo));
            Nombre = nombreNuevo;
        }

        var apellidoNuevo = apellido.Trim();
        if (apellidoNuevo != Apellido)
        {
            cambios.Add(new CambioDeCampo("apellido", Apellido, apellidoNuevo));
            Apellido = apellidoNuevo;
        }

        var telefonoNuevo = telefono.Trim();
        if (telefonoNuevo != Telefono)
        {
            cambios.Add(new CambioDeCampo("telefono", Telefono, telefonoNuevo));
            Telefono = telefonoNuevo;
            TelefonoNormalizado = NormalizarTelefono(telefonoNuevo);
        }

        var emailNuevo = VacioANulo(email)?.ToLowerInvariant();
        if (emailNuevo != Email)
        {
            cambios.Add(new CambioDeCampo("email", Email, emailNuevo));
            Email = emailNuevo;
        }

        var direccionNueva = VacioANulo(direccion);
        if (direccionNueva != Direccion)
        {
            cambios.Add(new CambioDeCampo("direccion", Direccion, direccionNueva));
            Direccion = direccionNueva;
        }

        var observacionesNuevas = VacioANulo(observaciones);
        if (observacionesNuevas != Observaciones)
        {
            cambios.Add(new CambioDeCampo("observaciones", Observaciones, observacionesNuevas));
            Observaciones = observacionesNuevas;
        }

        return cambios;
    }

    private static string? VacioANulo(string? valor) => string.IsNullOrWhiteSpace(valor) ? null : valor.Trim();
}
