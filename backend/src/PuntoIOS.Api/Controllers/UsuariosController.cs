using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PuntoIOS.Application.Common.Pagination;
using PuntoIOS.Application.Features.Identidad;
using PuntoIOS.Application.Features.Identidad.ActualizarUsuario;
using PuntoIOS.Application.Features.Identidad.ListarUsuarios;
using PuntoIOS.Application.Features.Identidad.RegistrarUsuario;
using PuntoIOS.Domain.Identidad;

namespace PuntoIOS.Api.Controllers;

public sealed record RegistrarUsuarioRequest(string Email, string Nombre, Guid RolId);

public sealed record ActualizarUsuarioRequest(Guid? RolId, bool? Activo);

[ApiController]
[Route("api/v1/usuarios")]
public sealed class UsuariosController(
    ListarUsuariosHandler listar,
    RegistrarUsuarioHandler registrar,
    ActualizarUsuarioHandler actualizar) : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = Permisos.UsuariosVer)]
    public async Task<ActionResult<PagedResult<UsuarioDto>>> Listar(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = PageRequest.DefaultPageSize,
        CancellationToken ct = default) =>
        Ok(await listar.HandleAsync(new PageRequest(page, pageSize), ct));

    [HttpPost]
    [Authorize(Policy = Permisos.UsuariosGestionar)]
    public async Task<ActionResult<UsuarioDto>> Registrar(RegistrarUsuarioRequest request, CancellationToken ct)
    {
        var resultado = await registrar.HandleAsync(
            new RegistrarUsuarioCommand(request.Email, request.Nombre, request.RolId), ct);

        return resultado.EsExitoso
            ? StatusCode(StatusCodes.Status201Created, resultado.Valor)
            : resultado.Error!.ToProblem();
    }

    [HttpPatch("{id:guid}")]
    [Authorize(Policy = Permisos.UsuariosGestionar)]
    public async Task<ActionResult<UsuarioDto>> Actualizar(Guid id, ActualizarUsuarioRequest request, CancellationToken ct)
    {
        var resultado = await actualizar.HandleAsync(new ActualizarUsuarioCommand(id, request.RolId, request.Activo), ct);
        return resultado.EsExitoso ? Ok(resultado.Valor) : resultado.Error!.ToProblem();
    }
}
