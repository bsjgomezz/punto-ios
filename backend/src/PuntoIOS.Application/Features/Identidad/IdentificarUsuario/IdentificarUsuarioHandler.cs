using PuntoIOS.Application.Common.Security;
using PuntoIOS.Application.Features.Identidad.Puertos;
using PuntoIOS.Domain.Identidad;

namespace PuntoIOS.Application.Features.Identidad.IdentificarUsuario;

public enum EstadoIdentificacion
{
    Autorizado,
    NoRegistrado,
    Desactivado,
}

public sealed record UsuarioAutorizado(Guid Id, string Rol, IReadOnlyList<string> Permisos);

public sealed record ResultadoIdentificacion(EstadoIdentificacion Estado, UsuarioAutorizado? Usuario)
{
    public static ResultadoIdentificacion NoRegistrado { get; } = new(EstadoIdentificacion.NoRegistrado, null);

    public static ResultadoIdentificacion Desactivado { get; } = new(EstadoIdentificacion.Desactivado, null);
}

/// <summary>
/// Caso de uso: relaciona la identidad de Firebase con un usuario local registrado.
/// Primero busca por uid; si no está vinculado todavía, busca por email (solo si Firebase lo verificó)
/// y lo vincula. Quien no está registrado o está desactivado no obtiene permisos.
/// </summary>
public sealed class IdentificarUsuarioHandler(IUsuarioRepositorio usuarios, TimeProvider reloj)
{
    private static readonly TimeSpan IntervaloRegistroIngreso = TimeSpan.FromMinutes(15);

    public async Task<ResultadoIdentificacion> HandleAsync(CurrentUser identidad, CancellationToken ct)
    {
        var usuario = await usuarios.ObtenerPorFirebaseUidAsync(identidad.Uid, ct);

        if (usuario is null)
        {
            if (!identidad.EmailVerified || string.IsNullOrWhiteSpace(identidad.Email))
            {
                return ResultadoIdentificacion.NoRegistrado;
            }

            usuario = await usuarios.ObtenerPorEmailAsync(Usuario.NormalizarEmail(identidad.Email), ct);

            // Si ese email ya está vinculado a otro uid de Firebase, no se reasigna.
            if (usuario is null || usuario.FirebaseUid is not null)
            {
                return ResultadoIdentificacion.NoRegistrado;
            }

            if (!usuario.Activo)
            {
                return ResultadoIdentificacion.Desactivado;
            }

            usuario.VincularFirebase(identidad.Uid, reloj.GetUtcNow());
            await usuarios.GuardarCambiosAsync(ct);
        }
        else
        {
            if (!usuario.Activo)
            {
                return ResultadoIdentificacion.Desactivado;
            }

            // Se identifica en cada solicitud: el último ingreso se actualiza como mucho cada 15 minutos.
            var ahora = reloj.GetUtcNow();
            if (usuario.UltimoIngresoEn is null || ahora - usuario.UltimoIngresoEn > IntervaloRegistroIngreso)
            {
                usuario.RegistrarIngreso(ahora);
                await usuarios.GuardarCambiosAsync(ct);
            }
        }

        return new ResultadoIdentificacion(
            EstadoIdentificacion.Autorizado,
            new UsuarioAutorizado(usuario.Id, usuario.Rol.Nombre, usuario.Rol.CodigosDePermisos()));
    }
}
