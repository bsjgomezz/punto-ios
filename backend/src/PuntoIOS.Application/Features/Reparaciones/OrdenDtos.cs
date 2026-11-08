using PuntoIOS.Application.Features.Clientes;
using PuntoIOS.Application.Features.Clientes.Puertos;
using PuntoIOS.Application.Features.Dispositivos;
using PuntoIOS.Application.Features.Dispositivos.Puertos;
using PuntoIOS.Application.Features.Identidad.Puertos;
using PuntoIOS.Domain.Reparaciones;

namespace PuntoIOS.Application.Features.Reparaciones;

/// <summary>
/// Orden completa para el personal. El desbloqueo nunca viaja acá: solo si existe y de qué tipo es.
/// <c>EstadosDisponibles</c>: estados a los que se puede pasar desde el actual.
/// </summary>
public sealed record OrdenReparacionDto(
    Guid Id,
    long Numero,
    string Estado,
    ClienteResumenDto Cliente,
    DispositivoDto Equipo,
    string ProblemaInformado,
    IReadOnlyList<string> TiposReparacion,
    string? OtroTipoDescripcion,
    UsuarioResumen? Tecnico,
    DateOnly? FechaEstimadaEntrega,
    string? DesbloqueoTipo,
    string? CreadoPor,
    DateTimeOffset CreadoEn,
    ChecklistDto Checklist,
    bool NoEnciende,
    IReadOnlyList<string> EstadosDisponibles);

/// <summary>Checklist de ingreso. <c>Items</c>: código → resultado (solo los revisados).</summary>
public sealed record ChecklistDto(IReadOnlyDictionary<string, bool> Items, int? SaludBateria, string? Notas);

public sealed record FotoDto(Guid Id, DateTimeOffset CreadoEn);

/// <summary>Fila de los listados de órdenes.</summary>
public sealed record OrdenResumenDto(
    Guid Id,
    long Numero,
    string Estado,
    Guid ClienteId,
    string Cliente,
    string Equipo,
    string? Tecnico,
    DateOnly? FechaEstimadaEntrega,
    DateTimeOffset CreadoEn);

/// <summary>Arma los DTO buscando en una sola consulta por tipo los clientes, equipos y usuarios involucrados.</summary>
internal sealed class ArmadorDeOrdenes(
    IClienteRepositorio clientes,
    IDispositivoRepositorio dispositivos,
    IDirectorioDeUsuarios directorio)
{
    public async Task<IReadOnlyList<OrdenReparacionDto>> CompletasAsync(IReadOnlyList<OrdenReparacion> ordenes, CancellationToken ct)
    {
        var (porCliente, porEquipo, nombres) = await BuscarAsync(ordenes, ct);
        return ordenes
            .Select(o => new OrdenReparacionDto(
                o.Id,
                o.Numero,
                o.Estado,
                ClienteResumenDto.Desde(porCliente[o.ClienteId]),
                DispositivoDto.Desde(porEquipo[o.DispositivoId]),
                o.ProblemaInformado,
                o.TiposReparacion,
                o.OtroTipoDescripcion,
                o.TecnicoUsuarioId is { } t ? new UsuarioResumen(t, nombres.GetValueOrDefault(t, "Usuario")) : null,
                o.FechaEstimadaEntrega,
                o.DesbloqueoTipo,
                o.CreadoPorUsuarioId is { } c ? nombres.GetValueOrDefault(c) : null,
                o.CreadoEn,
                new ChecklistDto(o.ChecklistItems.ToDictionary(i => i.Codigo, i => i.Valor), o.SaludBateria, o.NotasChecklist),
                o.NoEnciende,
                MaquinaDeEstados.SiguientesEstados(o.Estado)))
            .ToList();
    }

    public async Task<IReadOnlyList<OrdenResumenDto>> ResumenesAsync(IReadOnlyList<OrdenReparacion> ordenes, CancellationToken ct)
    {
        var (porCliente, porEquipo, nombres) = await BuscarAsync(ordenes, ct);
        return ordenes
            .Select(o =>
            {
                var cliente = porCliente[o.ClienteId];
                var equipo = porEquipo[o.DispositivoId];
                return new OrdenResumenDto(
                    o.Id,
                    o.Numero,
                    o.Estado,
                    cliente.Id,
                    $"{cliente.Apellido}, {cliente.Nombre}",
                    $"{equipo.Marca} {equipo.Modelo}",
                    o.TecnicoUsuarioId is { } t ? nombres.GetValueOrDefault(t) : null,
                    o.FechaEstimadaEntrega,
                    o.CreadoEn);
            })
            .ToList();
    }

    private async Task<(Dictionary<Guid, Domain.Clientes.Cliente>, Dictionary<Guid, Domain.Clientes.Dispositivo>, IReadOnlyDictionary<Guid, string>)>
        BuscarAsync(IReadOnlyList<OrdenReparacion> ordenes, CancellationToken ct)
    {
        var porCliente = (await clientes.ObtenerPorIdsAsync(ordenes.Select(o => o.ClienteId).Distinct(), ct))
            .ToDictionary(c => c.Id);
        var porEquipo = (await dispositivos.ObtenerPorIdsAsync(ordenes.Select(o => o.DispositivoId).Distinct(), ct))
            .ToDictionary(d => d.Id);
        var usuarios = ordenes
            .SelectMany(o => new[] { o.TecnicoUsuarioId, o.CreadoPorUsuarioId })
            .Where(id => id is not null)
            .Select(id => id!.Value)
            .Distinct();
        var nombres = await directorio.ObtenerNombresAsync(usuarios, ct);
        return (porCliente, porEquipo, nombres);
    }
}
