using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PuntoIOS.Application.Common.Pagination;
using PuntoIOS.Application.Features.Auditoria;
using PuntoIOS.Application.Features.Clientes.CrearCliente;
using PuntoIOS.Application.Features.Identidad.Puertos;
using PuntoIOS.Application.Features.Reparaciones;
using PuntoIOS.Application.Features.Reparaciones.ConsultarOrdenes;
using PuntoIOS.Application.Features.Reparaciones.Fotos;
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
    string? DesbloqueoValor = null,
    ChecklistRequest? Checklist = null);

/// <summary>Checklist de ingreso: <c>Items</c> es código → true/false (lo no revisado no se manda).</summary>
public sealed record ChecklistRequest(Dictionary<string, bool>? Items, int? SaludBateria, string? Notas);

public sealed record TipoReparacionDto(string Codigo, string Nombre);

[ApiController]
[Route("api/v1/reparaciones")]
public sealed class ReparacionesController(
    RecibirEquipoHandler recibir,
    ObtenerOrdenHandler obtener,
    ListarOrdenesHandler listar,
    ListarTecnicosHandler tecnicos,
    VerDesbloqueoHandler verDesbloqueo,
    HistorialOrdenHandler historial,
    AgregarFotoHandler agregarFoto,
    ListarFotosHandler listarFotos,
    ObtenerFotoHandler obtenerFoto) : ControllerBase
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
                request.DesbloqueoValor,
                request.Checklist is { } c
                    ? new ChecklistIngreso(
                        (c.Items ?? []).Select(i => new ItemChecklist(i.Key, i.Value)).ToList(), c.SaludBateria, c.Notas)
                    : null),
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

    /// <summary>Sube una foto del equipo (JPG, PNG o WebP, hasta 10 MB; hasta 10 por orden).</summary>
    [HttpPost("{id:guid}/fotos")]
    [Authorize(Policy = Permisos.ReparacionesCrear)]
    [RequestSizeLimit(FotoOrden.TamanoMaximoBytes + 1024 * 1024)]
    [RequestFormLimits(MultipartBodyLengthLimit = FotoOrden.TamanoMaximoBytes + 1024 * 1024)]
    public async Task<ActionResult<FotoDto>> AgregarFoto(Guid id, IFormFile? foto, CancellationToken ct)
    {
        if (foto is null)
        {
            return new Application.Common.Results.Error(
                Application.Common.Results.TipoError.Validacion, "foto_obligatoria", "Elegí una foto.").ToProblem();
        }

        await using var contenido = foto.OpenReadStream();
        var resultado = await agregarFoto.HandleAsync(id, contenido, foto.Length, ct);
        return resultado.EsExitoso ? Ok(resultado.Valor) : resultado.Error!.ToProblem();
    }

    [HttpGet("{id:guid}/fotos")]
    [Authorize(Policy = Permisos.ReparacionesVer)]
    public async Task<ActionResult<IReadOnlyList<FotoDto>>> Fotos(Guid id, CancellationToken ct)
    {
        var resultado = await listarFotos.HandleAsync(id, ct);
        return resultado.EsExitoso ? Ok(resultado.Valor) : resultado.Error!.ToProblem();
    }

    [HttpGet("{id:guid}/fotos/{fotoId:guid}")]
    [Authorize(Policy = Permisos.ReparacionesVer)]
    public async Task<IActionResult> Foto(Guid id, Guid fotoId, CancellationToken ct)
    {
        var resultado = await obtenerFoto.HandleAsync(id, fotoId, ct);
        if (!resultado.EsExitoso)
        {
            return resultado.Error!.ToProblem();
        }

        Response.Headers.XContentTypeOptions = "nosniff";
        Response.Headers.CacheControl = "private, max-age=3600";
        return File(resultado.Valor!.Contenido, resultado.Valor.TipoContenido);
    }
}
