using Microsoft.EntityFrameworkCore;
using PuntoIOS.Application.Common.Pagination;
using PuntoIOS.Application.Features.Identidad.Puertos;
using PuntoIOS.Domain.Identidad;

namespace PuntoIOS.Infrastructure.Persistence;

internal sealed class UsuarioRepositorio(PuntoIosDbContext db) : IUsuarioRepositorio
{
    private IQueryable<Usuario> ConRol => db.Usuarios.Include(u => u.Rol).ThenInclude(r => r.Permisos);

    public Task<Usuario?> ObtenerPorFirebaseUidAsync(string uid, CancellationToken ct) =>
        ConRol.FirstOrDefaultAsync(u => u.FirebaseUid == uid, ct);

    public Task<Usuario?> ObtenerPorEmailAsync(string emailNormalizado, CancellationToken ct) =>
        ConRol.FirstOrDefaultAsync(u => u.Email == emailNormalizado, ct);

    public Task<Usuario?> ObtenerPorIdAsync(Guid id, CancellationToken ct) =>
        ConRol.FirstOrDefaultAsync(u => u.Id == id, ct);

    public async Task<PagedResult<Usuario>> ListarAsync(PageRequest pagina, CancellationToken ct)
    {
        var total = await db.Usuarios.CountAsync(ct);
        var items = await ConRol
            .OrderBy(u => u.Nombre).ThenBy(u => u.Email)
            .Skip(pagina.Skip).Take(pagina.PageSize)
            .ToListAsync(ct);

        return new PagedResult<Usuario>(items, pagina.Page, pagina.PageSize, total);
    }

    public Task<int> ContarActivosConPermisoAsync(string permiso, CancellationToken ct) =>
        db.Usuarios.CountAsync(
            u => u.Activo && db.RolesPermisos.Any(rp => rp.RolId == u.RolId && rp.PermisoCodigo == permiso),
            ct);

    public void Agregar(Usuario usuario) => db.Usuarios.Add(usuario);

    public Task GuardarCambiosAsync(CancellationToken ct) => db.SaveChangesAsync(ct);
}

internal sealed class RolRepositorio(PuntoIosDbContext db) : IRolRepositorio
{
    public async Task<IReadOnlyList<Rol>> ListarAsync(CancellationToken ct) =>
        await db.Roles.Include(r => r.Permisos).OrderBy(r => r.Nombre).ToListAsync(ct);

    public Task<Rol?> ObtenerPorIdAsync(Guid id, CancellationToken ct) =>
        db.Roles.Include(r => r.Permisos).FirstOrDefaultAsync(r => r.Id == id, ct);
}
