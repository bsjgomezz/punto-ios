namespace PuntoIOS.Application.Common.Pagination;

/// <summary>
/// Parámetros de paginación de toda colección de la API.
/// Los valores inválidos se normalizan: página mínima 1, tamaño por defecto 20 y máximo 100.
/// </summary>
public sealed class PageRequest
{
    public const int DefaultPageSize = 20;
    public const int MaxPageSize = 100;

    public PageRequest(int page = 1, int pageSize = DefaultPageSize)
    {
        Page = page < 1 ? 1 : page;
        PageSize = pageSize < 1 ? DefaultPageSize : Math.Min(pageSize, MaxPageSize);
    }

    public int Page { get; }

    public int PageSize { get; }

    /// <summary>Cantidad de registros a omitir (acotada a <see cref="int.MaxValue"/>).</summary>
    public int Skip => (int)Math.Min((long)(Page - 1) * PageSize, int.MaxValue);
}
