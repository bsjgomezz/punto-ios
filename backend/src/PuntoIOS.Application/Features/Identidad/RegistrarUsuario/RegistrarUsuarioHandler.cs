using PuntoIOS.Application.Common.Results;
using PuntoIOS.Application.Common.Validation;
using PuntoIOS.Application.Features.Identidad.Puertos;
using PuntoIOS.Domain.Identidad;

namespace PuntoIOS.Application.Features.Identidad.RegistrarUsuario;

public sealed record RegistrarUsuarioCommand(string Email, string Nombre, Guid RolId);

/// <summary>Da de alta un usuario por email. Podrá ingresar cuando inicie sesión con ese email verificado.</summary>
public sealed class RegistrarUsuarioHandler(IUsuarioRepositorio usuarios, IRolRepositorio roles, TimeProvider reloj)
{
    public async Task<Resultado<UsuarioDto>> HandleAsync(RegistrarUsuarioCommand comando, CancellationToken ct)
    {
        var email = Usuario.NormalizarEmail(comando.Email);
        if (!EmailValidator.EsValido(email))
        {
            return Resultado<UsuarioDto>.Falla(TipoError.Validacion, "email_invalido", "El email no es válido.");
        }

        if (string.IsNullOrWhiteSpace(comando.Nombre))
        {
            return Resultado<UsuarioDto>.Falla(TipoError.Validacion, "nombre_obligatorio", "El nombre es obligatorio.");
        }

        var rol = await roles.ObtenerPorIdAsync(comando.RolId, ct);
        if (rol is null)
        {
            return Resultado<UsuarioDto>.Falla(TipoError.Validacion, "rol_inexistente", "El rol indicado no existe.");
        }

        if (await usuarios.ObtenerPorEmailAsync(email, ct) is not null)
        {
            return Resultado<UsuarioDto>.Falla(TipoError.Conflicto, "email_duplicado", "Ya existe un usuario con ese email.");
        }

        var usuario = Usuario.Registrar(email, comando.Nombre, rol, reloj.GetUtcNow());
        usuarios.Agregar(usuario);
        await usuarios.GuardarCambiosAsync(ct);

        return Resultado<UsuarioDto>.Ok(UsuarioDto.Desde(usuario));
    }
}
