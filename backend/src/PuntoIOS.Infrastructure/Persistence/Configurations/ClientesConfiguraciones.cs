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

internal sealed class DispositivoConfiguration : IEntityTypeConfiguration<Dispositivo>
{
    public void Configure(EntityTypeBuilder<Dispositivo> builder)
    {
        builder.ToTable("dispositivos");
        builder.HasKey(d => d.Id);
        builder.Property(d => d.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(d => d.LocalId).HasColumnName("local_id");
        builder.Property(d => d.ClienteId).HasColumnName("cliente_id");
        builder.Property(d => d.Marca).HasColumnName("marca").HasMaxLength(Dispositivo.LargoMaximoMarca).IsRequired();
        builder.Property(d => d.Modelo).HasColumnName("modelo").HasMaxLength(Dispositivo.LargoMaximoModelo).IsRequired();
        builder.Property(d => d.Almacenamiento).HasColumnName("almacenamiento").HasMaxLength(Dispositivo.LargoMaximoAlmacenamiento);
        builder.Property(d => d.Color).HasColumnName("color").HasMaxLength(Dispositivo.LargoMaximoColor);
        builder.Property(d => d.ImeiSerie).HasColumnName("imei_serie").HasMaxLength(Dispositivo.MaximoCaracteresImei);
        builder.Property(d => d.Observaciones).HasColumnName("observaciones").HasMaxLength(Dispositivo.LargoMaximoObservaciones);
        builder.Property(d => d.CreadoPorUsuarioId).HasColumnName("creado_por_usuario_id");
        builder.Property(d => d.CreadoEn).HasColumnName("creado_en");
        builder.Ignore(d => d.ImeiSerieEnmascarado);

        // Un IMEI no puede repetirse dentro del local: la base lo garantiza aunque dos personas carguen a la vez.
        builder.HasIndex(d => new { d.LocalId, d.ImeiSerie })
            .IsUnique()
            .HasFilter("imei_serie IS NOT NULL");
        builder.HasIndex(d => new { d.LocalId, d.ClienteId });

        builder.HasOne<Local>().WithMany().HasForeignKey(d => d.LocalId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Cliente>().WithMany().HasForeignKey(d => d.ClienteId).OnDelete(DeleteBehavior.Restrict);
    }
}
