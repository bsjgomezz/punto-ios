using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PuntoIOS.Application.Common.Pagination;
using PuntoIOS.Application.Features.Auditoria;
using PuntoIOS.Application.Features.Clientes.CrearCliente;
using PuntoIOS.Application.Features.Identidad.Puertos;
using PuntoIOS.Application.Features.Reparaciones;
using PuntoIOS.Application.Features.Reparaciones.ConsultarOrdenes;
using PuntoIOS.Application.Features.Reparaciones.RecibirEquipo;
using PuntoIOS.Application.Features.Reparaciones.VerDesbloqueo;
using PuntoIOS.Domain.Identidad;
using PuntoIOS.Domain.Reparaciones;

namespace PuntoIOS.Api.Controllers;

/// <summary>Equipo que entra al taller: uno ya registrado (<c>DispositivoId</c>) o uno nuevo (<c>EquipoNuevo</c>).</summary>
public sealed record RecibirEquipoRequest(
    Guid ClienteId,
    Guid? DispositivoId,
    EquipoNuevoRequest? EquipoNuevo,
    string? ProblemaInformado,
    IReadOnlyList<string>? TiposReparacion,
    string? OtroTipoDescripcion = null,
    Guid? TecnicoId = null,
    DateOnly? FechaEstimadaEntrega = null,
    string? DesbloqueoTipo = null,
    string? DesbloqueoValor = null);

public sealed record TipoReparacionDto(string Codigo, string Nombre);

[ApiController]
[Route("api/v1/reparaciones")]
public sealed class ReparacionesController(
    RecibirEquipoHandler recibir,
    ObtenerOrdenHandler obtener,
    ListarOrdenesHandler listar,
    ListarTecnicosHandler tecnicos,
    VerDesbloqueoHandler verDesbloqueo,
    HistorialOrdenHandler historial) : ControllerBase
{
    /// <summary>Órdenes de la más nueva a la más vieja. Con <c>clienteId</c>, solo las de ese cliente.</summary>
    [HttpGet]
    [Authorize(Policy = Permisos.ReparacionesVer)]
    public async Task<ActionResult<PagedResult<OrdenResumenDto>>> Listar(
        [FromQuery] Guid? clienteId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = PageRequest.DefaultPageSize,
        CancellationToken ct = default) =>
        Ok(await listar.HandleAsync(clienteId, new PageRequest(page, pageSize), ct));

    [HttpGet("{id:guid}")]
    [Authorize(Policy = Permisos.ReparacionesVer)]
    public async Task<ActionResult<OrdenReparacionDto>> Obtener(Guid id, CancellationToken ct)
    {
        var resultado = await obtener.HandleAsync(id, ct);
        return resultado.EsExitoso ? Ok(resultado.Valor) : resultado.Error!.ToProblem();
    }

    /// <summary>Recibe un equipo y crea la orden (y el equipo, si es nuevo) en una sola operación.</summary>
    [HttpPost]
    [Authorize(Policy = Permisos.ReparacionesCrear)]
    public async Task<ActionResult<OrdenReparacionDto>> Recibir(RecibirEquipoRequest request, CancellationToken ct)
    {
        var resultado = await recibir.HandleAsync(
            new RecibirEquipoCommand(
                request.ClienteId,
                request.DispositivoId,
                request.EquipoNuevo is { } e ? new EquipoNuevo(e.Marca, e.Modelo, e.Almacenamiento, e.Color, e.ImeiSerie) : null,
                request.ProblemaInformado,
                request.TiposReparacion,
                request.OtroTipoDescripcion,
                request.TecnicoId,
                request.FechaEstimadaEntrega,
                request.DesbloqueoTipo,
                request.DesbloqueoValor),
            ct);

        return resultado.EsExitoso
            ? CreatedAtAction(nameof(Obtener), new { id = resultado.Valor!.Id }, resultado.Valor)
            : resultado.Error!.ToProblem();
    }

    /// <summary>Tipos de reparación que se pueden marcar al recibir un equipo.</summary>
    [HttpGet("tipos")]
    [Authorize(Policy = Permisos.ReparacionesVer)]
    public ActionResult<IReadOnlyList<TipoReparacionDto>> Tipos() =>
        Ok(TiposDeReparacion.Catalogo.Select(t => new TipoReparacionDto(t.Key, t.Value)).ToList());

    /// <summary>Personas a las que se les puede asignar una reparación.</summary>
    [HttpGet("tecnicos")]
    [Authorize(Policy = Permisos.ReparacionesCrear)]
    public async Task<ActionResult<IReadOnlyList<UsuarioResumen>>> Tecnicos(CancellationToken ct) =>
        Ok(await tecnicos.HandleAsync(ct));

    /// <summary>Código o patrón de desbloqueo. Es POST porque deja registrado quién lo vio.</summary>
    [HttpPost("{id:guid}/desbloqueo")]
    [Authorize(Policy = Permisos.ReparacionesVerDesbloqueo)]
    public async Task<ActionResult<DesbloqueoDto>> VerDesbloqueo(Guid id, CancellationToken ct)
    {
        var resultado = await verDesbloqueo.HandleAsync(id, ct);
        return resultado.EsExitoso ? Ok(resultado.Valor) : resultado.Error!.ToProblem();
    }

    [HttpGet("{id:guid}/historial")]
    [Authorize(Policy = Permisos.ReparacionesVer)]
    public async Task<ActionResult<PagedResult<RegistroHistorialDto>>> Historial(
        Guid id,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = PageRequest.DefaultPageSize,
        CancellationToken ct = default)
    {
        var resultado = await historial.HandleAsync(id, new PageRequest(page, pageSize), ct);
        return resultado.EsExitoso ? Ok(resultado.Valor) : resultado.Error!.ToProblem();
    }
}
