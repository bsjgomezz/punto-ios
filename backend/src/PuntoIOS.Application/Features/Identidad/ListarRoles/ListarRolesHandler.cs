using PuntoIOS.Application.Features.Identidad.Puertos;

namespace PuntoIOS.Application.Features.Identidad.ListarRoles;

public sealed class ListarRolesHandler(IRolRepositorio roles)
{
    public async Task<IReadOnlyList<RolDto>> HandleAsync(CancellationToken ct) =>
        (await roles.ListarAsync(ct)).Select(RolDto.Desde).ToList();
}
