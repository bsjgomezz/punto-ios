using Microsoft.EntityFrameworkCore;
using PuntoIOS.Domain.Identidad;

namespace PuntoIOS.Infrastructure.Persistence;

public sealed class PuntoIosDbContext(DbContextOptions<PuntoIosDbContext> options) : DbContext(options)
{
    public DbSet<Usuario> Usuarios => Set<Usuario>();

    public DbSet<Rol> Roles => Set<Rol>();

    public DbSet<Permiso> Permisos => Set<Permiso>();

    public DbSet<RolPermiso> RolesPermisos => Set<RolPermiso>();

    protected override void OnModelCreating(ModelBuilder modelBuilder) =>
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PuntoIosDbContext).Assembly);
}
