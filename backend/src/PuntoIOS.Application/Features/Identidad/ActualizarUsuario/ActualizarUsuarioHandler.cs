using PuntoIOS.Application.Common.Results;
using PuntoIOS.Application.Features.Identidad.Puertos;
using PuntoIOS.Domain.Identidad;

namespace PuntoIOS.Application.Features.Identidad.ActualizarUsuario;

/// <summary>Cambios opcionales: solo se aplican los campos informados.</summary>
public sealed record ActualizarUsuarioCommand(Guid UsuarioId, Guid? RolId, bool? Activo);

/// <summary>
/// Cambia el rol de un usuario o lo activa/desactiva. Nunca permite dejar el sistema sin ningún
/// usuario activo que pueda gestionar usuarios.
/// </summary>
public sealed class ActualizarUsuarioHandler(IUsuarioRepositorio usuarios, IRolRepositorio roles)
{
    public async Task<Resultado<UsuarioDto>> HandleAsync(ActualizarUsuarioCommand comando, CancellationToken ct)
    {
        var usuario = await usuarios.ObtenerPorIdAsync(comando.UsuarioId, ct);
        if (usuario is null)
        {
            return Resultado<UsuarioDto>.Falla(TipoError.NoEncontrado, "usuario_inexistente", "El usuario no existe.");
        }

        var rolNuevo = usuario.Rol;
        if (comando.RolId is { } rolId && rolId != usuario.RolId)
        {
            var rol = await roles.ObtenerPorIdAsync(rolId, ct);
            if (rol is null)
            {
                return Resultado<UsuarioDto>.Falla(TipoError.Validacion, "rol_inexistente", "El rol indicado no existe.");
            }

            rolNuevo = rol;
        }

        var activoNuevo = comando.Activo ?? usuario.Activo;

        var gestionabaAntes = usuario.Activo && usuario.Rol.Concede(Permisos.UsuariosGestionar);
        var gestionaDespues = activoNuevo && rolNuevo.Concede(Permisos.UsuariosGestionar);

        if (gestionabaAntes && !gestionaDespues
            && await usuarios.ContarActivosConPermisoAsync(Permisos.UsuariosGestionar, ct) <= 1)
        {
            return Resultado<UsuarioDto>.Falla(
                TipoError.Conflicto,
                "ultimo_administrador",
                "No se puede dejar el sistema sin un administrador activo.");
        }

        if (rolNuevo.Id != usuario.RolId)
        {
            usuario.CambiarRol(rolNuevo);
        }

        if (activoNuevo) usuario.Activar(); else usuario.Desactivar();

        await usuarios.GuardarCambiosAsync(ct);
        return Resultado<UsuarioDto>.Ok(UsuarioDto.Desde(usuario));
    }
}
