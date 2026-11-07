using Microsoft.EntityFrameworkCore;
using PuntoIOS.Application.Common.Multilocal;
using PuntoIOS.Domain.Auditoria;
using PuntoIOS.Domain.Clientes;
using PuntoIOS.Domain.Identidad;
using PuntoIOS.Domain.Locales;

namespace PuntoIOS.Infrastructure.Persistence;

public sealed class PuntoIosDbContext(DbContextOptions<PuntoIosDbContext> options, ILocalActual localActual)
    : DbContext(options)
{
    public DbSet<Usuario> Usuarios => Set<Usuario>();

    public DbSet<Rol> Roles => Set<Rol>();

    public DbSet<Permiso> Permisos => Set<Permiso>();

    public DbSet<RolPermiso> RolesPermisos => Set<RolPermiso>();

    public DbSet<Local> Locales => Set<Local>();

    public DbSet<Cliente> Clientes => Set<Cliente>();

    public DbSet<Dispositivo> Dispositivos => Set<Dispositivo>();

    public DbSet<RegistroAuditoria> Auditoria => Set<RegistroAuditoria>();

    /// <summary>Local de la solicitud actual; lo usa el filtro global de las entidades operativas.</summary>
    private Guid LocalActualId => localActual.LocalId;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PuntoIosDbContext).Assembly);

        // Filtro por local aplicado de forma centralizada a toda entidad operativa.
        modelBuilder.Entity<Cliente>().HasQueryFilter(c => c.LocalId == LocalActualId);
        modelBuilder.Entity<Dispositivo>().HasQueryFilter(d => d.LocalId == LocalActualId);
        modelBuilder.Entity<RegistroAuditoria>().HasQueryFilter(r => r.LocalId == LocalActualId);
    }
}
