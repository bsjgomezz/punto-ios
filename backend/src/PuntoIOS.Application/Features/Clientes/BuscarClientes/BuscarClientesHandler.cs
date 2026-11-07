using PuntoIOS.Application.Common.Pagination;
using PuntoIOS.Application.Features.Clientes.Puertos;

namespace PuntoIOS.Application.Features.Clientes.BuscarClientes;

public sealed class BuscarClientesHandler(IClienteRepositorio clientes)
{
    public async Task<PagedResult<ClienteDto>> HandleAsync(string? texto, PageRequest pagina, CancellationToken ct)
    {
        var resultado = await clientes.BuscarAsync(texto?.Trim(), pagina, ct);
        return new PagedResult<ClienteDto>(
            resultado.Items.Select(ClienteDto.Desde).ToList(),
            resultado.Page,
            resultado.PageSize,
            resultado.TotalCount);
    }
}
