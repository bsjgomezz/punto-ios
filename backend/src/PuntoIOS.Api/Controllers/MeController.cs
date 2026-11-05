using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PuntoIOS.Application.Features.Identidad.ObtenerUsuarioActual;

namespace PuntoIOS.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/me")]
public sealed class MeController(ObtenerUsuarioActualHandler handler) : ControllerBase
{
    /// <summary>Identidad del usuario autenticado (según su Firebase ID Token).</summary>
    [HttpGet]
    public ActionResult<UsuarioActualDto> Get()
    {
        var user = handler.Handle();
        return user is null ? Unauthorized() : Ok(user);
    }
}
