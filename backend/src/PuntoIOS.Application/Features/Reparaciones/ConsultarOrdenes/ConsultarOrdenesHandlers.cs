using PuntoIOS.Application.Common.Pagination;
using PuntoIOS.Application.Common.Results;
using PuntoIOS.Application.Features.Clientes.Puertos;
using PuntoIOS.Application.Features.Dispositivos.Puertos;
using PuntoIOS.Application.Features.Identidad.Puertos;
using PuntoIOS.Application.Features.Reparaciones.Puertos;
using PuntoIOS.Domain.Identidad;

namespace PuntoIOS.Application.Features.Reparaciones.ConsultarOrdenes;

public sealed class ObtenerOrdenHandler(
    IOrdenReparacionRepositorio ordenes,
    IClienteRepositorio clientes,
    IDispositivoRepositorio dispositivos,
    IDirectorioDeUsuarios directorio)
{
    public async Task<Resultado<OrdenReparacionDto>> HandleAsync(Guid id, CancellationToken ct)
    {
        var orden = await ordenes.ObtenerPorIdAsync(id, ct);
        if (orden is null)
        {
            return Resultado<OrdenReparacionDto>.Falla(TipoError.NoEncontrado, "orden_inexistente", "La orden no existe.");
        }

        var armador = new ArmadorDeOrdenes(ordenes, clientes, dispositivos, directorio);
        return Resultado<OrdenReparacionDto>.Ok((await armador.CompletasAsync([orden], ct))[0]);
    }
}

public sealed class ListarOrdenesHandler(
    IOrdenReparacionRepositorio ordenes,
    IClienteRepositorio clientes,
    IDispositivoRepositorio dispositivos,
    IDirectorioDeUsuarios directorio)
{
    public async Task<PagedResult<OrdenResumenDto>> HandleAsync(FiltroDeOrdenes filtro, PageRequest pagina, CancellationToken ct)
    {
        var resultado = await ordenes.ListarAsync(filtro, pagina, ct);
        var armador = new ArmadorDeOrdenes(ordenes, clientes, dispositivos, directorio);
        var items = await armador.ResumenesAsync(resultado.Items, ct);
        return new PagedResult<OrdenResumenDto>(items, resultado.Page, resultado.PageSize, resultado.TotalCount);
    }
}

/// <summary>Usuarios a los que se les puede asignar una reparación (activos y con permiso para reparar).</summary>
public sealed class ListarTecnicosHandler(IDirectorioDeUsuarios directorio)
{
    public Task<IReadOnlyList<UsuarioResumen>> HandleAsync(CancellationToken ct) =>
        directorio.ListarActivosConPermisoAsync(Permisos.ReparacionesReparar, ct);
}

public sealed class HistorialOrdenHandler(
    IOrdenReparacionRepositorio ordenes,
    Auditoria.Puertos.IAuditoriaRepositorio auditoria,
    IDirectorioDeUsuarios directorio)
{
    public async Task<Resultado<PagedResult<Auditoria.RegistroHistorialDto>>> HandleAsync(
        Guid ordenId, PageRequest pagina, CancellationToken ct)
    {
        if (await ordenes.ObtenerPorIdAsync(ordenId, ct) is null)
        {
            return Resultado<PagedResult<Auditoria.RegistroHistorialDto>>.Falla(
                TipoError.NoEncontrado, "orden_inexistente", "La orden no existe.");
        }

        var registros = await auditoria.ListarAsync(Domain.Auditoria.EntidadesAuditadas.Reparacion, ordenId, pagina, ct);
        return Resultado<PagedResult<Auditoria.RegistroHistorialDto>>.Ok(
            await Auditoria.HistorialDeAuditoria.ArmarAsync(registros, directorio, ct));
    }
}
