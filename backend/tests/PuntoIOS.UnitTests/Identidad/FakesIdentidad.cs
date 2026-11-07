using PuntoIOS.Application.Common.Pagination;
using PuntoIOS.Application.Features.Identidad.Puertos;
using PuntoIOS.Domain.Identidad;

namespace PuntoIOS.UnitTests.Identidad;

internal sealed class RelojFijo(DateTimeOffset ahora) : TimeProvider
{
    public DateTimeOffset Ahora { get; set; } = ahora;

    public override DateTimeOffset GetUtcNow() => Ahora;
}

internal sealed class UsuarioRepositorioEnMemoria : IUsuarioRepositorio
{
    public List<Usuario> Usuarios { get; } = [];

    public int Guardados { get; private set; }

    public Task<Usuario?> ObtenerPorFirebaseUidAsync(string uid, CancellationToken ct) =>
        Task.FromResult(Usuarios.FirstOrDefault(u => u.FirebaseUid == uid));

    public Task<Usuario?> ObtenerPorEmailAsync(string emailNormalizado, CancellationToken ct) =>
        Task.FromResult(Usuarios.FirstOrDefault(u => u.Email == emailNormalizado));

    public Task<Usuario?> ObtenerPorIdAsync(Guid id, CancellationToken ct) =>
        Task.FromResult(Usuarios.FirstOrDefault(u => u.Id == id));

    public Task<PagedResult<Usuario>> ListarAsync(PageRequest pagina, CancellationToken ct) =>
        Task.FromResult(new PagedResult<Usuario>(
            Usuarios.Skip(pagina.Skip).Take(pagina.PageSize).ToList(), pagina.Page, pagina.PageSize, Usuarios.Count));

    public Task<int> ContarActivosConPermisoAsync(string permiso, CancellationToken ct) =>
        Task.FromResult(Usuarios.Count(u => u.Activo && u.Rol.Concede(permiso)));

    public void Agregar(Usuario usuario) => Usuarios.Add(usuario);

    public Task GuardarCambiosAsync(CancellationToken ct)
    {
        Guardados++;
        return Task.CompletedTask;
    }
}

internal sealed class RolRepositorioEnMemoria(params Rol[] roles) : IRolRepositorio
{
    public Task<IReadOnlyList<Rol>> ListarAsync(CancellationToken ct) => Task.FromResult<IReadOnlyList<Rol>>(roles);

    public Task<Rol?> ObtenerPorIdAsync(Guid id, CancellationToken ct) =>
        Task.FromResult(roles.FirstOrDefault(r => r.Id == id));
}

internal static class Roles
{
    public static Rol Administrador()
    {
        var rol = new Rol(RolesDeSistema.AdministradorId, RolesDeSistema.Administrador, "Todo", true);
        foreach (var permiso in Permisos.Catalogo.Keys)
        {
            rol.Conceder(permiso);
        }

        return rol;
    }

    public static Rol Tecnico() => new(RolesDeSistema.TecnicoId, RolesDeSistema.Tecnico, "Repara", true);
}
