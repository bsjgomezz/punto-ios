namespace PuntoIOS.Domain.Reparaciones;

/// <summary>Foto del equipo tomada al recibirlo. El archivo vive en el almacén de archivos; acá solo su referencia.</summary>
public sealed class FotoOrden
{
    public const int MaximoPorOrden = 10;
    public const long TamanoMaximoBytes = 10 * 1024 * 1024;

    private FotoOrden()
    {
    }

    public Guid Id { get; private set; }

    public Guid LocalId { get; private set; }

    public Guid OrdenId { get; private set; }

    /// <summary>Ruta relativa dentro del almacén. La genera el sistema, nunca viene del nombre que subió el usuario.</summary>
    public string Ruta { get; private set; } = string.Empty;

    public string TipoContenido { get; private set; } = string.Empty;

    public long Tamano { get; private set; }

    public Guid? CreadoPorUsuarioId { get; private set; }

    public DateTimeOffset CreadoEn { get; private set; }

    public static FotoOrden Registrar(
        Guid localId, Guid ordenId, string extension, string tipoContenido, long tamano, Guid? creadoPorUsuarioId, DateTimeOffset ahora)
    {
        var id = Guid.CreateVersion7();
        return new FotoOrden
        {
            Id = id,
            LocalId = localId,
            OrdenId = ordenId,
            Ruta = $"reparaciones/{ordenId}/{id}.{extension}",
            TipoContenido = tipoContenido,
            Tamano = tamano,
            CreadoPorUsuarioId = creadoPorUsuarioId,
            CreadoEn = ahora,
        };
    }
}
