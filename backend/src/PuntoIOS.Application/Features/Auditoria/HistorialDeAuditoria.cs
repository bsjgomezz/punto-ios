using PuntoIOS.Application.Common.Pagination;
using PuntoIOS.Application.Features.Identidad.Puertos;
using PuntoIOS.Domain.Auditoria;

namespace PuntoIOS.Application.Features.Auditoria;

/// <summary>Una entrada del historial tal como la ve la interfaz: qué pasó, quién y cuándo.</summary>
public sealed record RegistroHistorialDto(
    Guid Id,
    string Accion,
    Guid? UsuarioId,
    string? Usuario,
    DateTimeOffset Fecha,
    IReadOnlyList<CambioDeCampo> Cambios);

internal static class HistorialDeAuditoria
{
    /// <summary>Agrega el nombre de quién hizo cada cambio, consultando al módulo Identidad en una sola llamada.</summary>
    public static async Task<PagedResult<RegistroHistorialDto>> ArmarAsync(
        PagedResult<RegistroAuditoria> registros, IDirectorioDeUsuarios directorio, CancellationToken ct)
    {
        var nombres = await directorio.ObtenerNombresAsync(
            registros.Items.Where(r => r.UsuarioId is not null).Select(r => r.UsuarioId!.Value).Distinct(), ct);

        var items = registros.Items
            .Select(r => new RegistroHistorialDto(
                r.Id,
                r.Accion,
                r.UsuarioId,
                r.UsuarioId is { } id && nombres.TryGetValue(id, out var nombre) ? nombre : null,
                r.Fecha,
                r.Cambios))
            .ToList();

        return new PagedResult<RegistroHistorialDto>(items, registros.Page, registros.PageSize, registros.TotalCount);
    }
}
