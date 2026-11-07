namespace PuntoIOS.Domain.Identidad;

/// <summary>
/// Usuario local del sistema. Firebase solo autentica; quién puede entrar y qué puede hacer se decide acá.
/// Se registra primero por email y se vincula a su <c>uid</c> de Firebase en el primer ingreso.
/// </summary>
public sealed class Usuario
{
    private Usuario()
    {
    }

    public Guid Id { get; private set; }

    /// <summary>Identificador de Firebase. Es null hasta el primer ingreso del usuario.</summary>
    public string? FirebaseUid { get; private set; }

    /// <summary>Email normalizado (minúsculas, sin espacios).</summary>
    public string Email { get; private set; } = string.Empty;

    public string Nombre { get; private set; } = string.Empty;

    public bool Activo { get; private set; }

    public Guid RolId { get; private set; }

    public Rol Rol { get; private set; } = null!;

    public DateTimeOffset CreadoEn { get; private set; }

    public DateTimeOffset? UltimoIngresoEn { get; private set; }

    public static string NormalizarEmail(string email) => (email ?? string.Empty).Trim().ToLowerInvariant();

    public static Usuario Registrar(string email, string nombre, Rol rol, DateTimeOffset ahora)
    {
        var emailNormalizado = NormalizarEmail(email);
        if (emailNormalizado.Length == 0)
        {
            throw new ArgumentException("El email es obligatorio.", nameof(email));
        }

        if (string.IsNullOrWhiteSpace(nombre))
        {
            throw new ArgumentException("El nombre es obligatorio.", nameof(nombre));
        }

        ArgumentNullException.ThrowIfNull(rol);

        return new Usuario
        {
            Id = Guid.CreateVersion7(),
            Email = emailNormalizado,
            Nombre = nombre.Trim(),
            Activo = true,
            RolId = rol.Id,
            Rol = rol,
            CreadoEn = ahora,
        };
    }

    /// <summary>Asocia la cuenta de Firebase. Una vez vinculada no puede cambiar a otro uid.</summary>
    public void VincularFirebase(string uid, DateTimeOffset ahora)
    {
        if (string.IsNullOrWhiteSpace(uid))
        {
            throw new ArgumentException("El uid es obligatorio.", nameof(uid));
        }

        if (FirebaseUid is not null && FirebaseUid != uid)
        {
            throw new InvalidOperationException("El usuario ya está vinculado a otra cuenta de Firebase.");
        }

        FirebaseUid = uid;
        UltimoIngresoEn = ahora;
    }

    public void RegistrarIngreso(DateTimeOffset ahora) => UltimoIngresoEn = ahora;

    public void CambiarRol(Rol rol)
    {
        ArgumentNullException.ThrowIfNull(rol);
        Rol = rol;
        RolId = rol.Id;
    }

    public void Activar() => Activo = true;

    public void Desactivar() => Activo = false;
}
