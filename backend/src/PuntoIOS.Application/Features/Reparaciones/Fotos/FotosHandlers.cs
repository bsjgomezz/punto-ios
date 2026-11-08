using PuntoIOS.Application.Common.Archivos;
using PuntoIOS.Application.Common.Multilocal;
using PuntoIOS.Application.Common.Results;
using PuntoIOS.Application.Common.Security;
using PuntoIOS.Application.Features.Auditoria.Puertos;
using PuntoIOS.Application.Features.Reparaciones.Puertos;
using PuntoIOS.Domain.Auditoria;
using PuntoIOS.Domain.Reparaciones;

namespace PuntoIOS.Application.Features.Reparaciones.Fotos;

/// <summary>
/// Agrega una foto del equipo a la orden. Solo JPEG, PNG o WebP (reconocidos por su contenido, no por el nombre),
/// hasta 10 MB y hasta 10 fotos por orden.
/// </summary>
public sealed class AgregarFotoHandler(
    IOrdenReparacionRepositorio ordenes,
    IAlmacenDeArchivos almacen,
    IAuditoriaRepositorio auditoria,
    ILocalActual local,
    ICurrentUserAccessor usuarioActual,
    TimeProvider reloj)
{
    public async Task<Resultado<FotoDto>> HandleAsync(Guid ordenId, Stream contenido, long tamano, CancellationToken ct)
    {
        if (await ordenes.ObtenerPorIdAsync(ordenId, ct) is null)
        {
            return Resultado<FotoDto>.Falla(TipoError.NoEncontrado, "orden_inexistente", "La orden no existe.");
        }

        if (tamano <= 0 || tamano > FotoOrden.TamanoMaximoBytes)
        {
            return Resultado<FotoDto>.Falla(TipoError.Validacion, "foto_demasiado_grande", "Cada foto puede pesar hasta 10 MB.");
        }

        if ((await ordenes.ListarFotosAsync(ordenId, ct)).Count >= FotoOrden.MaximoPorOrden)
        {
            return Resultado<FotoDto>.Falla(
                TipoError.Conflicto, "demasiadas_fotos", $"Cada orden admite hasta {FotoOrden.MaximoPorOrden} fotos.");
        }

        // Se lee entero (son 10 MB como máximo) para validar el formato real antes de guardarlo.
        using var memoria = new MemoryStream();
        await contenido.CopyToAsync(memoria, ct);
        if (memoria.Length > FotoOrden.TamanoMaximoBytes)
        {
            return Resultado<FotoDto>.Falla(TipoError.Validacion, "foto_demasiado_grande", "Cada foto puede pesar hasta 10 MB.");
        }

        var formato = FormatoDeImagen.Detectar(memoria.GetBuffer().AsSpan(0, (int)Math.Min(memoria.Length, FormatoDeImagen.BytesNecesarios)));
        if (formato is not { } f)
        {
            return Resultado<FotoDto>.Falla(TipoError.Validacion, "formato_no_admitido", "La foto tiene que ser JPG, PNG o WebP.");
        }

        var usuarioId = usuarioActual.GetCurrentUser()?.UsuarioId;
        var ahora = reloj.GetUtcNow();
        var foto = FotoOrden.Registrar(local.LocalId, ordenId, f.Extension, f.TipoContenido, memoria.Length, usuarioId, ahora);

        memoria.Position = 0;
        await almacen.GuardarAsync(foto.Ruta, memoria, ct);

        ordenes.AgregarFoto(foto);
        auditoria.Agregar(RegistroAuditoria.Registrar(
            local.LocalId, EntidadesAuditadas.Reparacion, ordenId, AccionesAuditadas.FotoAgregada, usuarioId, ahora));
        await ordenes.GuardarCambiosAsync(ct);

        return Resultado<FotoDto>.Ok(new FotoDto(foto.Id, foto.CreadoEn));
    }
}

public sealed class ListarFotosHandler(IOrdenReparacionRepositorio ordenes)
{
    public async Task<Resultado<IReadOnlyList<FotoDto>>> HandleAsync(Guid ordenId, CancellationToken ct)
    {
        if (await ordenes.ObtenerPorIdAsync(ordenId, ct) is null)
        {
            return Resultado<IReadOnlyList<FotoDto>>.Falla(TipoError.NoEncontrado, "orden_inexistente", "La orden no existe.");
        }

        var fotos = await ordenes.ListarFotosAsync(ordenId, ct);
        return Resultado<IReadOnlyList<FotoDto>>.Ok(fotos.Select(f => new FotoDto(f.Id, f.CreadoEn)).ToList());
    }
}

public sealed record ArchivoDeFoto(Stream Contenido, string TipoContenido);

public sealed class ObtenerFotoHandler(IOrdenReparacionRepositorio ordenes, IAlmacenDeArchivos almacen)
{
    public async Task<Resultado<ArchivoDeFoto>> HandleAsync(Guid ordenId, Guid fotoId, CancellationToken ct)
    {
        var foto = await ordenes.ObtenerFotoAsync(ordenId, fotoId, ct);
        var contenido = foto is null ? null : await almacen.AbrirAsync(foto.Ruta, ct);
        return foto is null || contenido is null
            ? Resultado<ArchivoDeFoto>.Falla(TipoError.NoEncontrado, "foto_inexistente", "La foto no existe.")
            : Resultado<ArchivoDeFoto>.Ok(new ArchivoDeFoto(contenido, foto.TipoContenido));
    }
}
