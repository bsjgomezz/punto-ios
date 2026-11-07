using Microsoft.EntityFrameworkCore;
using PuntoIOS.Application.Common.Pagination;
using PuntoIOS.Application.Features.Reparaciones.Puertos;
using PuntoIOS.Domain.Reparaciones;

namespace PuntoIOS.Infrastructure.Persistence;

internal sealed class OrdenReparacionRepositorio(PuntoIosDbContext db) : IOrdenReparacionRepositorio
{
    public Task<OrdenReparacion?> ObtenerPorIdAsync(Guid id, CancellationToken ct) =>
        db.OrdenesReparacion.FirstOrDefaultAsync(o => o.Id == id, ct);

    public async Task<PagedResult<OrdenReparacion>> ListarAsync(Guid? clienteId, PageRequest pagina, CancellationToken ct)
    {
        IQueryable<OrdenReparacion> consulta = db.OrdenesReparacion;
        if (clienteId is { } id)
        {
            consulta = consulta.Where(o => o.ClienteId == id);
        }

        var total = await consulta.CountAsync(ct);
        var items = await consulta
            .OrderByDescending(o => o.Numero)
            .Skip(pagina.Skip).Take(pagina.PageSize)
            .ToListAsync(ct);
        return new PagedResult<OrdenReparacion>(items, pagina.Page, pagina.PageSize, total);
    }

    public void Agregar(OrdenReparacion orden) => db.OrdenesReparacion.Add(orden);

    public Task GuardarCambiosAsync(CancellationToken ct) => db.SaveChangesAsync(ct);
}
