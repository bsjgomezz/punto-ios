using PuntoIOS.Application.Common.Results;
using PuntoIOS.Application.Features.Clientes.Puertos;

namespace PuntoIOS.Application.Features.Clientes.ObtenerCliente;

public sealed class ObtenerClienteHandler(IClienteRepositorio clientes)
{
    public async Task<Resultado<ClienteDto>> HandleAsync(Guid id, CancellationToken ct)
    {
        var cliente = await clientes.ObtenerPorIdAsync(id, ct);
        return cliente is null
            ? Resultado<ClienteDto>.Falla(TipoError.NoEncontrado, "cliente_inexistente", "El cliente no existe.")
            : Resultado<ClienteDto>.Ok(ClienteDto.Desde(cliente));
    }
}
