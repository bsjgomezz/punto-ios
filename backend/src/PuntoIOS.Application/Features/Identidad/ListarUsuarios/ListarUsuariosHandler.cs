using PuntoIOS.Application.Common.Pagination;
using PuntoIOS.Application.Features.Identidad.Puertos;

namespace PuntoIOS.Application.Features.Identidad.ListarUsuarios;

public sealed class ListarUsuariosHandler(IUsuarioRepositorio usuarios)
{
    public async Task<PagedResult<UsuarioDto>> HandleAsync(PageRequest pagina, CancellationToken ct)
    {
        var resultado = await usuarios.ListarAsync(pagina, ct);
        return new PagedResult<UsuarioDto>(
            resultado.Items.Select(UsuarioDto.Desde).ToList(),
            resultado.Page,
            resultado.PageSize,
            resultado.TotalCount);
    }
}
