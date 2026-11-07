using PuntoIOS.Application.Common.Pagination;
using PuntoIOS.Application.Common.Results;
using PuntoIOS.Application.Features.Auditoria;
using PuntoIOS.Application.Features.Auditoria.Puertos;
using PuntoIOS.Application.Features.Clientes.Puertos;
using PuntoIOS.Application.Features.Identidad.Puertos;
using PuntoIOS.Domain.Auditoria;

namespace PuntoIOS.Application.Features.Clientes.HistorialCliente;

public sealed class HistorialClienteHandler(
    IClienteRepositorio clientes,
    IAuditoriaRepositorio auditoria,
    IDirectorioDeUsuarios directorio)
{
    public async Task<Resultado<PagedResult<RegistroHistorialDto>>> HandleAsync(
        Guid clienteId, PageRequest pagina, CancellationToken ct)
    {
        if (await clientes.ObtenerPorIdAsync(clienteId, ct) is null)
        {
            return Resultado<PagedResult<RegistroHistorialDto>>.Falla(
                TipoError.NoEncontrado, "cliente_inexistente", "El cliente no existe.");
        }

        var registros = await auditoria.ListarAsync(EntidadesAuditadas.Cliente, clienteId, pagina, ct);
        return Resultado<PagedResult<RegistroHistorialDto>>.Ok(await HistorialDeAuditoria.ArmarAsync(registros, directorio, ct));
    }
}
