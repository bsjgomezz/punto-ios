using PuntoIOS.Application.Common.Archivos;

namespace PuntoIOS.Infrastructure.Archivos;

/// <summary>
/// Guarda los archivos en una carpeta del equipo donde corre la API (configurable con Archivos:Carpeta).
/// Por defecto usa la carpeta de datos del usuario, fuera del proyecto, para que no se borre al compilar.
/// Esa carpeta hay que incluirla en los backups.
/// </summary>
public sealed class AlmacenEnDisco : IAlmacenDeArchivos
{
    private readonly string _raiz;

    public AlmacenEnDisco(string? carpeta)
    {
        var elegida = string.IsNullOrWhiteSpace(carpeta)
            ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "PuntoIOS", "archivos")
            : carpeta;
        _raiz = Path.GetFullPath(elegida);
    }

    public async Task GuardarAsync(string ruta, Stream contenido, CancellationToken ct)
    {
        var destino = RutaCompleta(ruta);
        Directory.CreateDirectory(Path.GetDirectoryName(destino)!);

        // Se escribe a un temporal y se renombra: nunca queda un archivo a medio guardar.
        var temporal = destino + ".tmp";
        await using (var archivo = new FileStream(temporal, FileMode.Create, FileAccess.Write, FileShare.None, 81920, useAsync: true))
        {
            await contenido.CopyToAsync(archivo, ct);
        }

        File.Move(temporal, destino, overwrite: false);
    }

    public Task<Stream?> AbrirAsync(string ruta, CancellationToken ct)
    {
        var origen = RutaCompleta(ruta);
        Stream? stream = File.Exists(origen)
            ? new FileStream(origen, FileMode.Open, FileAccess.Read, FileShare.Read, 81920, useAsync: true)
            : null;
        return Task.FromResult(stream);
    }

    /// <summary>Resuelve la ruta y verifica que no se salga de la carpeta raíz.</summary>
    private string RutaCompleta(string ruta)
    {
        var completa = Path.GetFullPath(Path.Combine(_raiz, ruta));
        if (!completa.StartsWith(_raiz + Path.DirectorySeparatorChar, StringComparison.Ordinal))
        {
            throw new InvalidOperationException("Ruta de archivo fuera del almacén.");
        }

        return completa;
    }
}
