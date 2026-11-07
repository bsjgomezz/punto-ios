using PuntoIOS.Application.Common.Pagination;
using PuntoIOS.Application.Common.Results;
using PuntoIOS.Application.Features.Auditoria.Puertos;
using PuntoIOS.Application.Features.Clientes.Puertos;
using PuntoIOS.Application.Features.Identidad.Puertos;
using PuntoIOS.Domain.Auditoria;

namespace PuntoIOS.Application.Features.Clientes.HistorialCliente;

public sealed record RegistroHistorialDto(
    Guid Id,
    string Accion,
    Guid? UsuarioId,
    string? Usuario,
    DateTimeOffset Fecha,
    IReadOnlyList<CambioDeCampo> Cambios);

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
        var nombres = await directorio.ObtenerNombresAsync(
            registros.Items.Where(r => r.UsuarioId is not null).Select(r => r.UsuarioId!.Value).Distinct(), ct);

        var items = registros.Items
            .Select(r => new RegistroHistorialDto(
                r.Id,
                r.Accion,
                r.UsuarioId,
                r.UsuarioId is { } id && nombres.TryGetValue(id, out var nombre) ? nombre : null,
                r.Fecha,
                r.Cambios))
            .ToList();

        return Resultado<PagedResult<RegistroHistorialDto>>.Ok(
            new PagedResult<RegistroHistorialDto>(items, registros.Page, registros.PageSize, registros.TotalCount));
    }
}
