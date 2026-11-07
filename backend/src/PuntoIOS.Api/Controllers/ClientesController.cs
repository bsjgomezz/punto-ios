using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PuntoIOS.Application.Common.Pagination;
using PuntoIOS.Application.Features.Auditoria;
using PuntoIOS.Application.Features.Clientes;
using PuntoIOS.Application.Features.Clientes.ActualizarCliente;
using PuntoIOS.Application.Features.Clientes.BuscarClientes;
using PuntoIOS.Application.Features.Clientes.CrearCliente;
using PuntoIOS.Application.Features.Clientes.HistorialCliente;
using PuntoIOS.Application.Features.Clientes.ObtenerCliente;
using PuntoIOS.Domain.Identidad;

namespace PuntoIOS.Api.Controllers;

/// <summary>Equipo que trae el cliente; se guarda junto con el cliente.</summary>
public sealed record EquipoNuevoRequest(
    string? Marca,
    string? Modelo,
    string? Almacenamiento,
    string? Color,
    string? ImeiSerie);

public sealed record CrearClienteRequest(
    string? Nombre,
    string? Apellido,
    string? Telefono,
    string? Email,
    string? Direccion,
    string? Observaciones,
    bool PermitirDuplicado = false,
    EquipoNuevoRequest? Equipo = null);

public sealed record ActualizarClienteRequest(
    string? Nombre,
    string? Apellido,
    string? Telefono,
    string? Email,
    string? Direccion,
    string? Observaciones,
    bool PermitirDuplicado = false);

[ApiController]
[Route("api/v1/clientes")]
public sealed class ClientesController(
    BuscarClientesHandler buscar,
    ObtenerClienteHandler obtener,
    CrearClienteHandler crear,
    ActualizarClienteHandler actualizar,
    HistorialClienteHandler historial) : ControllerBase
{
    /// <summary>Lista paginada. <c>q</c> busca por nombre, apellido, email o teléfono.</summary>
    [HttpGet]
    [Authorize(Policy = Permisos.ClientesVer)]
    public async Task<ActionResult<PagedResult<ClienteDto>>> Buscar(
        [FromQuery] string? q,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = PageRequest.DefaultPageSize,
        CancellationToken ct = default) =>
        Ok(await buscar.HandleAsync(q, new PageRequest(page, pageSize), ct));

    [HttpGet("{id:guid}")]
    [Authorize(Policy = Permisos.ClientesVer)]
    public async Task<ActionResult<ClienteDto>> Obtener(Guid id, CancellationToken ct)
    {
        var resultado = await obtener.HandleAsync(id, ct);
        return resultado.EsExitoso ? Ok(resultado.Valor) : resultado.Error!.ToProblem();
    }

    [HttpPost]
    [Authorize(Policy = Permisos.ClientesCrear)]
    public async Task<ActionResult<ClienteDto>> Crear(CrearClienteRequest request, CancellationToken ct)
    {
        var resultado = await crear.HandleAsync(
            new CrearClienteCommand(
                request.Nombre,
                request.Apellido,
                request.Telefono,
                request.Email,
                request.Direccion,
                request.Observaciones,
                request.PermitirDuplicado,
                request.Equipo is { } e ? new EquipoNuevo(e.Marca, e.Modelo, e.Almacenamiento, e.Color, e.ImeiSerie) : null),
            ct);

        return resultado.EsExitoso
            ? CreatedAtAction(nameof(Obtener), new { id = resultado.Valor!.Id }, resultado.Valor)
            : resultado.Error!.ToProblem();
    }

    /// <summary>Reemplaza los datos editables del cliente. Registra en el historial lo que cambió.</summary>
    [HttpPut("{id:guid}")]
    [Authorize(Policy = Permisos.ClientesEditar)]
    public async Task<ActionResult<ClienteDto>> Actualizar(Guid id, ActualizarClienteRequest request, CancellationToken ct)
    {
        var resultado = await actualizar.HandleAsync(
            new ActualizarClienteCommand(
                id,
                request.Nombre,
                request.Apellido,
                request.Telefono,
                request.Email,
                request.Direccion,
                request.Observaciones,
                request.PermitirDuplicado),
            ct);

        return resultado.EsExitoso ? Ok(resultado.Valor) : resultado.Error!.ToProblem();
    }

    /// <summary>Historial de cambios del cliente, del más reciente al más antiguo.</summary>
    [HttpGet("{id:guid}/historial")]
    [Authorize(Policy = Permisos.ClientesVer)]
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
