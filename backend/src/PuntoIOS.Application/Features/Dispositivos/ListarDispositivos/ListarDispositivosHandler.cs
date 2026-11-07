using PuntoIOS.Application.Common.Results;
using PuntoIOS.Application.Features.Clientes.Puertos;
using PuntoIOS.Application.Features.Dispositivos.Puertos;

namespace PuntoIOS.Application.Features.Dispositivos.ListarDispositivos;

public sealed class ListarDispositivosHandler(IClienteRepositorio clientes, IDispositivoRepositorio dispositivos)
{
    public async Task<Resultado<IReadOnlyList<DispositivoDto>>> HandleAsync(Guid clienteId, CancellationToken ct)
    {
        if (await clientes.ObtenerPorIdAsync(clienteId, ct) is null)
        {
            return Resultado<IReadOnlyList<DispositivoDto>>.Falla(
                TipoError.NoEncontrado, "cliente_inexistente", "El cliente no existe.");
        }

        var lista = await dispositivos.ListarPorClienteAsync(clienteId, ct);
        return Resultado<IReadOnlyList<DispositivoDto>>.Ok(lista.Select(DispositivoDto.Desde).ToList());
    }
}
