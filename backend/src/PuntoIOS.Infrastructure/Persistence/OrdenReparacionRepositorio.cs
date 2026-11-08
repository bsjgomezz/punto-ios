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

    public void AgregarFoto(FotoOrden foto) => db.FotosOrden.Add(foto);

    public async Task<IReadOnlyList<FotoOrden>> ListarFotosAsync(Guid ordenId, CancellationToken ct) =>
        await db.FotosOrden.Where(f => f.OrdenId == ordenId).OrderBy(f => f.CreadoEn).ToListAsync(ct);

    public Task<FotoOrden?> ObtenerFotoAsync(Guid ordenId, Guid fotoId, CancellationToken ct) =>
        db.FotosOrden.FirstOrDefaultAsync(f => f.OrdenId == ordenId && f.Id == fotoId, ct);

    public Task GuardarCambiosAsync(CancellationToken ct) => db.SaveChangesAsync(ct);
}
