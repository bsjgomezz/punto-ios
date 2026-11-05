namespace PuntoIOS.Application.Common.Pagination;

/// <summary>Respuesta paginada: <c>{ items, page, pageSize, totalCount, totalPages }</c>.</summary>
public sealed record PagedResult<T>(IReadOnlyList<T> Items, int Page, int PageSize, int TotalCount)
{
    public int TotalPages => TotalCount == 0 ? 0 : (int)Math.Ceiling(TotalCount / (double)PageSize);
}
