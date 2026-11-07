using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PuntoIOS.Application.Common.Pagination;
using PuntoIOS.Application.Features.Auditoria;
using PuntoIOS.Application.Features.Dispositivos;
using PuntoIOS.Application.Features.Dispositivos.ActualizarDispositivo;
using PuntoIOS.Application.Features.Dispositivos.CrearDispositivo;
using PuntoIOS.Application.Features.Dispositivos.HistorialDispositivo;
using PuntoIOS.Application.Features.Dispositivos.ListarDispositivos;
using PuntoIOS.Application.Features.Dispositivos.ObtenerDispositivo;
using PuntoIOS.Application.Features.Dispositivos.RevelarImei;
using PuntoIOS.Domain.Identidad;

namespace PuntoIOS.Api.Controllers;

public sealed record CrearDispositivoRequest(
    string? Marca,
    string? Modelo,
    string? Almacenamiento,
    string? Color,
    string? ImeiSerie,
    string? Observaciones);

/// <summary><c>ActualizarImei</c> en falso deja el IMEI como está.</summary>
public sealed record ActualizarDispositivoRequest(
    string? Marca,
    string? Modelo,
    string? Almacenamiento,
    string? Color,
    string? Observaciones,
    bool ActualizarImei = false,
    string? ImeiSerie = null);

[ApiController]
[Route("api/v1")]
public sealed class DispositivosController(
    ListarDispositivosHandler listar,
    ObtenerDispositivoHandler obtener,
    CrearDispositivoHandler crear,
    ActualizarDispositivoHandler actualizar,
    RevelarImeiHandler revelarImei,
    HistorialDispositivoHandler historial) : ControllerBase
{
    /// <summary>Equipos del cliente, del más reciente al más antiguo. El IMEI va enmascarado.</summary>
    [HttpGet("clientes/{clienteId:guid}/dispositivos")]
    [Authorize(Policy = Permisos.DispositivosVer)]
    public async Task<ActionResult<IReadOnlyList<DispositivoDto>>> Listar(Guid clienteId, CancellationToken ct)
    {
        var resultado = await listar.HandleAsync(clienteId, ct);
        return resultado.EsExitoso ? Ok(resultado.Valor) : resultado.Error!.ToProblem();
    }

    [HttpPost("clientes/{clienteId:guid}/dispositivos")]
    [Authorize(Policy = Permisos.DispositivosCrear)]
    public async Task<ActionResult<DispositivoDto>> Crear(Guid clienteId, CrearDispositivoRequest request, CancellationToken ct)
    {
        var resultado = await crear.HandleAsync(
            new CrearDispositivoCommand(
                clienteId,
                request.Marca,
                request.Modelo,
                request.Almacenamiento,
                request.Color,
                request.ImeiSerie,
                request.Observaciones),
            ct);

        return resultado.EsExitoso
            ? CreatedAtAction(nameof(Obtener), new { id = resultado.Valor!.Id }, resultado.Valor)
            : resultado.Error!.ToProblem();
    }

    [HttpGet("dispositivos/{id:guid}")]
    [Authorize(Policy = Permisos.DispositivosVer)]
    public async Task<ActionResult<DispositivoDto>> Obtener(Guid id, CancellationToken ct)
    {
        var resultado = await obtener.HandleAsync(id, ct);
        return resultado.EsExitoso ? Ok(resultado.Valor) : resultado.Error!.ToProblem();
    }

    [HttpPut("dispositivos/{id:guid}")]
    [Authorize(Policy = Permisos.DispositivosEditar)]
    public async Task<ActionResult<DispositivoDto>> Actualizar(Guid id, ActualizarDispositivoRequest request, CancellationToken ct)
    {
        var resultado = await actualizar.HandleAsync(
            new ActualizarDispositivoCommand(
                id,
                request.Marca,
                request.Modelo,
                request.Almacenamiento,
                request.Color,
                request.Observaciones,
                request.ActualizarImei,
                request.ImeiSerie),
            ct);

        return resultado.EsExitoso ? Ok(resultado.Valor) : resultado.Error!.ToProblem();
    }

    /// <summary>IMEI o número de serie completo. Es POST porque deja registrado quién lo consultó.</summary>
    [HttpPost("dispositivos/{id:guid}/imei")]
    [Authorize(Policy = Permisos.DispositivosVerImei)]
    public async Task<ActionResult<ImeiDto>> RevelarImei(Guid id, CancellationToken ct)
    {
        var resultado = await revelarImei.HandleAsync(id, ct);
        return resultado.EsExitoso ? Ok(resultado.Valor) : resultado.Error!.ToProblem();
    }

    [HttpGet("dispositivos/{id:guid}/historial")]
    [Authorize(Policy = Permisos.DispositivosVer)]
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
