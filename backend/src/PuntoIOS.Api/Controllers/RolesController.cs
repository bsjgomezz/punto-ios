using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PuntoIOS.Application.Features.Identidad;
using PuntoIOS.Application.Features.Identidad.ListarRoles;
using PuntoIOS.Domain.Identidad;

namespace PuntoIOS.Api.Controllers;

[ApiController]
[Route("api/v1/roles")]
public sealed class RolesController(ListarRolesHandler listar) : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = Permisos.RolesVer)]
    public async Task<ActionResult<IReadOnlyList<RolDto>>> Listar(CancellationToken ct) =>
        Ok(await listar.HandleAsync(ct));
}
