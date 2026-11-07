using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PuntoIOS.Application.Common.Pagination;
using PuntoIOS.Application.Features.Clientes;
using PuntoIOS.Application.Features.Clientes.BuscarClientes;
using PuntoIOS.Application.Features.Clientes.CrearCliente;
using PuntoIOS.Application.Features.Clientes.ObtenerCliente;
using PuntoIOS.Domain.Identidad;

namespace PuntoIOS.Api.Controllers;

public sealed record CrearClienteRequest(
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
    CrearClienteHandler crear) : ControllerBase
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
                request.PermitirDuplicado),
            ct);

        return resultado.EsExitoso
            ? CreatedAtAction(nameof(Obtener), new { id = resultado.Valor!.Id }, resultado.Valor)
            : resultado.Error!.ToProblem();
    }
}
