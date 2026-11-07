using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PuntoIOS.Domain.Clientes;
using PuntoIOS.Domain.Locales;

namespace PuntoIOS.Infrastructure.Persistence.Configurations;

internal sealed class LocalConfiguration : IEntityTypeConfiguration<Local>
{
    public void Configure(EntityTypeBuilder<Local> builder)
    {
        builder.ToTable("locales");
        builder.HasKey(l => l.Id);
        builder.Property(l => l.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(l => l.Nombre).HasColumnName("nombre").HasMaxLength(100).IsRequired();

        builder.HasData(new Local(LocalesDeSistema.PrincipalId, LocalesDeSistema.PrincipalNombre));
    }
}

internal sealed class ClienteConfiguration : IEntityTypeConfiguration<Cliente>
{
    public void Configure(EntityTypeBuilder<Cliente> builder)
    {
        builder.ToTable("clientes");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(c => c.LocalId).HasColumnName("local_id");
        builder.Property(c => c.Nombre).HasColumnName("nombre").HasMaxLength(Cliente.LargoMaximoNombre).IsRequired();
        builder.Property(c => c.Apellido).HasColumnName("apellido").HasMaxLength(Cliente.LargoMaximoNombre).IsRequired();
        builder.Property(c => c.Telefono).HasColumnName("telefono").HasMaxLength(50).IsRequired();
        builder.Property(c => c.TelefonoNormalizado).HasColumnName("telefono_normalizado").HasMaxLength(20).IsRequired();
        builder.Property(c => c.Email).HasColumnName("email").HasMaxLength(Cliente.LargoMaximoEmail);
        builder.Property(c => c.Direccion).HasColumnName("direccion").HasMaxLength(Cliente.LargoMaximoDireccion);
        builder.Property(c => c.Observaciones).HasColumnName("observaciones").HasMaxLength(Cliente.LargoMaximoObservaciones);
        builder.Property(c => c.CreadoPorUsuarioId).HasColumnName("creado_por_usuario_id");
        builder.Property(c => c.CreadoEn).HasColumnName("creado_en");

        // Un teléfono puede repetirse a propósito (familiares), por eso el índice no es único.
        builder.HasIndex(c => new { c.LocalId, c.TelefonoNormalizado });
        builder.HasIndex(c => new { c.LocalId, c.Apellido, c.Nombre });

        builder.HasOne<Local>().WithMany().HasForeignKey(c => c.LocalId).OnDelete(DeleteBehavior.Restrict);
    }
}
