using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PuntoIOS.Domain.Clientes;
using PuntoIOS.Domain.Identidad;
using PuntoIOS.Domain.Locales;
using PuntoIOS.Domain.Reparaciones;

namespace PuntoIOS.Infrastructure.Persistence.Configurations;

internal sealed class OrdenReparacionConfiguration : IEntityTypeConfiguration<OrdenReparacion>
{
    /// <summary>Secuencia de PostgreSQL que numera las órdenes (1, 2, 3…). Nunca repite un número.</summary>
    public const string SecuenciaNumero = "ordenes_reparacion_numero_seq";

    public void Configure(EntityTypeBuilder<OrdenReparacion> builder)
    {
        builder.ToTable("ordenes_reparacion");
        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(o => o.LocalId).HasColumnName("local_id");
        builder.Property(o => o.Numero).HasColumnName("numero").HasDefaultValueSql($"nextval('{SecuenciaNumero}')");
        builder.Property(o => o.ClienteId).HasColumnName("cliente_id");
        builder.Property(o => o.DispositivoId).HasColumnName("dispositivo_id");
        builder.Property(o => o.ProblemaInformado).HasColumnName("problema_informado")
            .HasMaxLength(OrdenReparacion.LargoMaximoProblema).IsRequired();
        builder.Property(o => o.TiposReparacion).HasColumnName("tipos_reparacion").IsRequired();
        builder.Property(o => o.OtroTipoDescripcion).HasColumnName("otro_tipo_descripcion")
            .HasMaxLength(OrdenReparacion.LargoMaximoOtroTipo);
        builder.Property(o => o.TecnicoUsuarioId).HasColumnName("tecnico_usuario_id");
        builder.Property(o => o.FechaEstimadaEntrega).HasColumnName("fecha_estimada_entrega");
        builder.Property(o => o.Estado).HasColumnName("estado").HasMaxLength(40).IsRequired();
        builder.Property(o => o.DesbloqueoTipo).HasColumnName("desbloqueo_tipo").HasMaxLength(20);
        builder.Property(o => o.DesbloqueoCifrado).HasColumnName("desbloqueo_cifrado").HasMaxLength(1000);
        builder.Property(o => o.CreadoPorUsuarioId).HasColumnName("creado_por_usuario_id");
        builder.Property(o => o.CreadoEn).HasColumnName("creado_en");
        builder.Property(o => o.SaludBateria).HasColumnName("salud_bateria");
        builder.Property(o => o.NotasChecklist).HasColumnName("notas_checklist").HasMaxLength(ItemsDeChecklist.LargoMaximoNotas);
        builder.OwnsMany(o => o.ChecklistItems, items => items.ToJson("checklist_ingreso"));
        builder.Ignore(o => o.TieneDesbloqueo);
        builder.Ignore(o => o.NoEnciende);

        builder.HasIndex(o => o.Numero).IsUnique();
        builder.HasIndex(o => new { o.LocalId, o.CreadoEn });
        builder.HasIndex(o => new { o.LocalId, o.ClienteId });
        builder.HasIndex(o => new { o.LocalId, o.TecnicoUsuarioId, o.Estado });

        builder.HasOne<Local>().WithMany().HasForeignKey(o => o.LocalId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Cliente>().WithMany().HasForeignKey(o => o.ClienteId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Dispositivo>().WithMany().HasForeignKey(o => o.DispositivoId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Usuario>().WithMany().HasForeignKey(o => o.TecnicoUsuarioId).OnDelete(DeleteBehavior.Restrict);
    }
}

internal sealed class FotoOrdenConfiguration : IEntityTypeConfiguration<FotoOrden>
{
    public void Configure(EntityTypeBuilder<FotoOrden> builder)
    {
        builder.ToTable("fotos_orden");
        builder.HasKey(f => f.Id);
        builder.Property(f => f.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(f => f.LocalId).HasColumnName("local_id");
        builder.Property(f => f.OrdenId).HasColumnName("orden_id");
        builder.Property(f => f.Ruta).HasColumnName("ruta").HasMaxLength(300).IsRequired();
        builder.Property(f => f.TipoContenido).HasColumnName("tipo_contenido").HasMaxLength(50).IsRequired();
        builder.Property(f => f.Tamano).HasColumnName("tamano");
        builder.Property(f => f.CreadoPorUsuarioId).HasColumnName("creado_por_usuario_id");
        builder.Property(f => f.CreadoEn).HasColumnName("creado_en");

        builder.HasIndex(f => new { f.LocalId, f.OrdenId, f.CreadoEn });
        builder.HasOne<Local>().WithMany().HasForeignKey(f => f.LocalId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<OrdenReparacion>().WithMany().HasForeignKey(f => f.OrdenId).OnDelete(DeleteBehavior.Restrict);
    }
}
