using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PuntoIOS.Domain.Identidad;

namespace PuntoIOS.Infrastructure.Persistence.Configurations;

internal sealed class PermisoConfiguration : IEntityTypeConfiguration<Permiso>
{
    public void Configure(EntityTypeBuilder<Permiso> builder)
    {
        builder.ToTable("permisos");
        builder.HasKey(p => p.Codigo);
        builder.Property(p => p.Codigo).HasColumnName("codigo").HasMaxLength(100);
        builder.Property(p => p.Descripcion).HasColumnName("descripcion").HasMaxLength(300).IsRequired();

        builder.HasData(Permisos.Catalogo.Select(p => new Permiso(p.Key, p.Value)));
    }
}

internal sealed class RolConfiguration : IEntityTypeConfiguration<Rol>
{
    public void Configure(EntityTypeBuilder<Rol> builder)
    {
        builder.ToTable("roles");
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(r => r.Nombre).HasColumnName("nombre").HasMaxLength(80).IsRequired();
        builder.Property(r => r.Descripcion).HasColumnName("descripcion").HasMaxLength(300).IsRequired();
        builder.Property(r => r.EsDeSistema).HasColumnName("es_de_sistema");
        builder.HasIndex(r => r.Nombre).IsUnique();

        builder.HasMany(r => r.Permisos).WithOne().HasForeignKey(rp => rp.RolId).OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(r => r.Permisos).UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasData(
            new Rol(RolesDeSistema.AdministradorId, RolesDeSistema.Administrador, "Acceso total al sistema", true),
            new Rol(RolesDeSistema.RecepcionVentasId, RolesDeSistema.RecepcionVentas, "Atiende clientes, recibe equipos y registra ventas", true),
            new Rol(RolesDeSistema.TecnicoId, RolesDeSistema.Tecnico, "Diagnostica y repara los equipos asignados", true));
    }
}

internal sealed class RolPermisoConfiguration : IEntityTypeConfiguration<RolPermiso>
{
    public void Configure(EntityTypeBuilder<RolPermiso> builder)
    {
        builder.ToTable("roles_permisos");
        builder.HasKey(rp => new { rp.RolId, rp.PermisoCodigo });
        builder.Property(rp => rp.RolId).HasColumnName("rol_id");
        builder.Property(rp => rp.PermisoCodigo).HasColumnName("permiso_codigo").HasMaxLength(100);
        builder.HasOne<Permiso>().WithMany().HasForeignKey(rp => rp.PermisoCodigo).OnDelete(DeleteBehavior.Restrict);

        // Permisos iniciales por rol (editables luego). El Administrador tiene todo el catálogo.
        var recepcion = new[]
        {
            Permisos.ClientesVer, Permisos.ClientesCrear, Permisos.ClientesEditar,
            Permisos.DispositivosVer, Permisos.DispositivosCrear, Permisos.DispositivosEditar, Permisos.DispositivosVerImei,
        };

        // El técnico ve el IMEI completo para chequear bloqueos (iCloud, denuncias); cada consulta queda registrada.
        var tecnico = new[] { Permisos.ClientesVer, Permisos.DispositivosVer, Permisos.DispositivosVerImei };

        builder.HasData(
            Permisos.Catalogo.Keys.Select(c => new RolPermiso(RolesDeSistema.AdministradorId, c))
                .Concat(recepcion.Select(c => new RolPermiso(RolesDeSistema.RecepcionVentasId, c)))
                .Concat(tecnico.Select(c => new RolPermiso(RolesDeSistema.TecnicoId, c))));
    }
}

internal sealed class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.ToTable("usuarios");
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(u => u.FirebaseUid).HasColumnName("firebase_uid").HasMaxLength(128);
        builder.Property(u => u.Email).HasColumnName("email").HasMaxLength(254).IsRequired();
        builder.Property(u => u.Nombre).HasColumnName("nombre").HasMaxLength(150).IsRequired();
        builder.Property(u => u.Activo).HasColumnName("activo");
        builder.Property(u => u.RolId).HasColumnName("rol_id");
        builder.Property(u => u.CreadoEn).HasColumnName("creado_en");
        builder.Property(u => u.UltimoIngresoEn).HasColumnName("ultimo_ingreso_en");

        builder.HasIndex(u => u.Email).IsUnique();
        builder.HasIndex(u => u.FirebaseUid).IsUnique(); // varios NULL permitidos en PostgreSQL

        builder.HasOne(u => u.Rol).WithMany().HasForeignKey(u => u.RolId).OnDelete(DeleteBehavior.Restrict);
        builder.Navigation(u => u.Rol).AutoInclude();
    }
}
