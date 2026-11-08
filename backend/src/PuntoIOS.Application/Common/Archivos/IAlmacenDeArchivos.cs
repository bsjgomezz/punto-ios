namespace PuntoIOS.Application.Common.Archivos;

/// <summary>Almacén de archivos (fotos). Las rutas son relativas y las genera siempre el sistema.</summary>
public interface IAlmacenDeArchivos
{
    Task GuardarAsync(string ruta, Stream contenido, CancellationToken ct);

    /// <summary>Abre el archivo para leerlo, o null si no existe.</summary>
    Task<Stream?> AbrirAsync(string ruta, CancellationToken ct);
}

/// <summary>Reconoce el formato real de una imagen por sus primeros bytes (no confía en el nombre ni en el tipo declarado).</summary>
public static class FormatoDeImagen
{
    public const int BytesNecesarios = 12;

    public static (string TipoContenido, string Extension)? Detectar(ReadOnlySpan<byte> cabecera)
    {
        if (cabecera.Length >= 3 && cabecera[0] == 0xFF && cabecera[1] == 0xD8 && cabecera[2] == 0xFF)
        {
            return ("image/jpeg", "jpg");
        }

        if (cabecera.Length >= 8 && cabecera[..8].SequenceEqual(new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }))
        {
            return ("image/png", "png");
        }

        if (cabecera.Length >= 12
            && cabecera[..4].SequenceEqual("RIFF"u8)
            && cabecera[8..12].SequenceEqual("WEBP"u8))
        {
            return ("image/webp", "webp");
        }

        return null;
    }
}
