using Microsoft.EntityFrameworkCore;
using PuntoIOS.Application.Common.Pagination;
using PuntoIOS.Application.Features.Reparaciones.Puertos;
using PuntoIOS.Domain.Reparaciones;

namespace PuntoIOS.Infrastructure.Persistence;

internal sealed class OrdenReparacionRepositorio(PuntoIosDbContext db) : IOrdenReparacionRepositorio
{
    public Task<OrdenReparacion?> ObtenerPorIdAsync(Guid id, CancellationToken ct) =>
        db.OrdenesReparacion.FirstOrDefaultAsync(o => o.Id == id, ct);

    public async Task<PagedResult<OrdenReparacion>> ListarAsync(FiltroDeOrdenes filtro, PageRequest pagina, CancellationToken ct)
    {
        IQueryable<OrdenReparacion> consulta = db.OrdenesReparacion;
        if (filtro.ClienteId is { } clienteId)
        {
            consulta = consulta.Where(o => o.ClienteId == clienteId);
        }

        if (filtro.TecnicoId is { } tecnicoId)
        {
            consulta = consulta.Where(o => o.TecnicoUsuarioId == tecnicoId);
        }

        if (filtro.SoloActivas)
        {
            consulta = consulta.Where(o => o.Estado != EstadosDeReparacion.Entregado && o.Estado != EstadosDeReparacion.Cancelado);
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

    public void AgregarVersionPresupuesto(VersionDePresupuesto version) => db.VersionesPresupuesto.Add(version);

    public async Task<IReadOnlyList<VersionDePresupuesto>> ListarVersionesPresupuestoAsync(Guid ordenId, CancellationToken ct) =>
        await db.VersionesPresupuesto.Where(v => v.OrdenId == ordenId).OrderByDescending(v => v.Numero).ToListAsync(ct);

    public Task<VersionDePresupuesto?> ObtenerVersionVigentePresupuestoAsync(Guid ordenId, CancellationToken ct) =>
        db.VersionesPresupuesto.Where(v => v.OrdenId == ordenId).OrderByDescending(v => v.Numero).FirstOrDefaultAsync(ct);

    public async Task<IReadOnlyList<VersionDePresupuesto>> ObtenerVersionesVigentesPresupuestoAsync(
        IEnumerable<Guid> ordenIds, CancellationToken ct)
    {
        var ids = ordenIds.Distinct().ToList();
        if (ids.Count == 0)
        {
            return [];
        }

        // Vigente = no hay otra versión de la misma orden con número mayor.
        return await db.VersionesPresupuesto
            .Where(v => ids.Contains(v.OrdenId)
                && !db.VersionesPresupuesto.Any(otra => otra.OrdenId == v.OrdenId && otra.Numero > v.Numero))
            .ToListAsync(ct);
    }

    public Task GuardarCambiosAsync(CancellationToken ct) => db.SaveChangesAsync(ct);
}
