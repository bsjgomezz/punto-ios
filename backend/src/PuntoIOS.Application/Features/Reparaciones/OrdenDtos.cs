using PuntoIOS.Application.Features.Clientes;
using PuntoIOS.Application.Features.Clientes.Puertos;
using PuntoIOS.Application.Features.Dispositivos;
using PuntoIOS.Application.Features.Dispositivos.Puertos;
using PuntoIOS.Application.Features.Identidad.Puertos;
using PuntoIOS.Application.Features.Reparaciones.Puertos;
using PuntoIOS.Domain.Reparaciones;

namespace PuntoIOS.Application.Features.Reparaciones;

/// <summary>
/// Orden completa para el personal. El desbloqueo nunca viaja acá: solo si existe y de qué tipo es.
/// <c>EstadosDisponibles</c>: estados a los que se puede pasar desde el actual con el cambio de estado genérico
/// (no incluye "aprobado" ni "presupuesto_rechazado", que solo se alcanzan respondiendo el presupuesto).
/// Es la vista interna del taller: incluye <c>NotasInternas</c>, que son privadas. Un futuro DTO público para el
/// cliente (portal de seguimiento, S18) NO debe reutilizar este record ni copiar <c>NotasInternas</c>.
/// <c>PresupuestoVigente</c>: la versión de presupuesto de número más alto, o null si todavía no se presupuestó.
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
    IReadOnlyList<string> EstadosDisponibles,
    string? Pruebas,
    string? Diagnostico,
    string? Mediciones,
    IReadOnlyList<string> ComponentesAfectados,
    string? TrabajoRealizado,
    string? NotasInternas,
    VersionDePresupuestoDto? PresupuestoVigente);

/// <summary>Checklist de ingreso. <c>Items</c>: código → resultado (solo los revisados).</summary>
public sealed record ChecklistDto(IReadOnlyDictionary<string, bool> Items, int? SaludBateria, string? Notas);

public sealed record FotoDto(Guid Id, DateTimeOffset CreadoEn);

public sealed record ItemPresupuestoDto(string Descripcion, string Tipo, decimal Monto);

/// <summary>
/// Una versión del presupuesto de una orden. <c>CreadoPor</c>: nombre de quien la creó.
/// <c>RespuestaAprobada</c>: true = aprobada, false = rechazada, null = sin respuesta todavía.
/// <c>RespondidoPor</c>: nombre de quien del staff registró la respuesta.
/// </summary>
public sealed record VersionDePresupuestoDto(
    Guid Id,
    int Numero,
    IReadOnlyList<ItemPresupuestoDto> Items,
    decimal Total,
    string? CreadoPor,
    DateTimeOffset CreadoEn,
    bool? RespuestaAprobada,
    DateTimeOffset? RespondidoEn,
    string? RespondidoPor)
{
    public static VersionDePresupuestoDto Desde(VersionDePresupuesto version, IReadOnlyDictionary<Guid, string> nombres) =>
        new(
            version.Id,
            version.Numero,
            version.Items.Select(i => new ItemPresupuestoDto(i.Descripcion, i.Tipo, i.Monto)).ToList(),
            version.Total,
            version.CreadoPorUsuarioId is { } c ? nombres.GetValueOrDefault(c) : null,
            version.CreadoEn,
            version.RespuestaAprobada,
            version.RespondidoEn,
            version.RespondidoPorUsuarioId is { } r ? nombres.GetValueOrDefault(r) : null);

    /// <summary>Usuarios cuyos nombres hacen falta para armar el DTO de esta versión (quién la creó y quién registró la respuesta).</summary>
    public static IEnumerable<Guid?> UsuariosDe(VersionDePresupuesto version) =>
        [version.CreadoPorUsuarioId, version.RespondidoPorUsuarioId];
}

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

/// <summary>
/// Arma los DTO buscando en una sola consulta por tipo los clientes, equipos, presupuestos vigentes y usuarios involucrados.
/// </summary>
internal sealed class ArmadorDeOrdenes(
    IOrdenReparacionRepositorio repositorioDeOrdenes,
    IClienteRepositorio clientes,
    IDispositivoRepositorio dispositivos,
    IDirectorioDeUsuarios directorio)
{
    public async Task<IReadOnlyList<OrdenReparacionDto>> CompletasAsync(IReadOnlyList<OrdenReparacion> ordenes, CancellationToken ct)
    {
        var vigentes = (await repositorioDeOrdenes.ObtenerVersionesVigentesPresupuestoAsync(ordenes.Select(o => o.Id), ct))
            .ToDictionary(v => v.OrdenId);
        var (porCliente, porEquipo, nombres) = await BuscarAsync(ordenes, vigentes.Values.SelectMany(v => VersionDePresupuestoDto.UsuariosDe(v)), ct);
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
                // Sin "aprobado" ni "presupuesto_rechazado": a esos estados solo se llega respondiendo el presupuesto
                // (POST .../presupuestos/respuesta), y el cambio de estado genérico los rechaza.
                MaquinaDeEstados.SiguientesEstados(o.Estado)
                    .Where(e => e is not (EstadosDeReparacion.Aprobado or EstadosDeReparacion.PresupuestoRechazado))
                    .ToList(),
                o.Pruebas,
                o.Diagnostico,
                o.Mediciones,
                o.ComponentesAfectados,
                o.TrabajoRealizado,
                o.NotasInternas, // Solo para el personal: no pasar NotasInternas a un DTO público (S18).
                vigentes.TryGetValue(o.Id, out var vigente) ? VersionDePresupuestoDto.Desde(vigente, nombres) : null))
            .ToList();
    }

    public async Task<IReadOnlyList<OrdenResumenDto>> ResumenesAsync(IReadOnlyList<OrdenReparacion> ordenes, CancellationToken ct)
    {
        var (porCliente, porEquipo, nombres) = await BuscarAsync(ordenes, [], ct);
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
        BuscarAsync(IReadOnlyList<OrdenReparacion> ordenes, IEnumerable<Guid?> otrosUsuarios, CancellationToken ct)
    {
        var porCliente = (await clientes.ObtenerPorIdsAsync(ordenes.Select(o => o.ClienteId).Distinct(), ct))
            .ToDictionary(c => c.Id);
        var porEquipo = (await dispositivos.ObtenerPorIdsAsync(ordenes.Select(o => o.DispositivoId).Distinct(), ct))
            .ToDictionary(d => d.Id);
        var usuarios = ordenes
            .SelectMany(o => new[] { o.TecnicoUsuarioId, o.CreadoPorUsuarioId })
            .Concat(otrosUsuarios)
            .Where(id => id is not null)
            .Select(id => id!.Value)
            .Distinct();
        var nombres = await directorio.ObtenerNombresAsync(usuarios, ct);
        return (porCliente, porEquipo, nombres);
    }
}
