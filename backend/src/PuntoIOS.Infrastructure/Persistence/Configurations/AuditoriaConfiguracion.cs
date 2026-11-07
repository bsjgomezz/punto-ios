using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PuntoIOS.Domain.Auditoria;

namespace PuntoIOS.Infrastructure.Persistence.Configurations;

internal sealed class RegistroAuditoriaConfiguration : IEntityTypeConfiguration<RegistroAuditoria>
{
    public void Configure(EntityTypeBuilder<RegistroAuditoria> builder)
    {
        builder.ToTable("auditoria");
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(r => r.LocalId).HasColumnName("local_id");
        builder.Property(r => r.Entidad).HasColumnName("entidad").HasMaxLength(50).IsRequired();
        builder.Property(r => r.EntidadId).HasColumnName("entidad_id");
        builder.Property(r => r.Accion).HasColumnName("accion").HasMaxLength(50).IsRequired();
        builder.Property(r => r.UsuarioId).HasColumnName("usuario_id");
        builder.Property(r => r.Fecha).HasColumnName("fecha");

        // Los campos que cambiaron se guardan como JSON (jsonb) dentro del mismo registro.
        builder.OwnsMany(r => r.Cambios, cambios => cambios.ToJson("cambios"));

        builder.HasIndex(r => new { r.LocalId, r.Entidad, r.EntidadId, r.Fecha });
    }
}
