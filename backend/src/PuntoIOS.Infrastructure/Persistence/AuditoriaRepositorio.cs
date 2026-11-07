using Microsoft.EntityFrameworkCore;
using PuntoIOS.Application.Common.Pagination;
using PuntoIOS.Application.Features.Auditoria.Puertos;
using PuntoIOS.Application.Features.Identidad.Puertos;
using PuntoIOS.Domain.Auditoria;

namespace PuntoIOS.Infrastructure.Persistence;

internal sealed class AuditoriaRepositorio(PuntoIosDbContext db) : IAuditoriaRepositorio
{
    public void Agregar(RegistroAuditoria registro) => db.Auditoria.Add(registro);

    public async Task<PagedResult<RegistroAuditoria>> ListarAsync(
        string entidad, Guid entidadId, PageRequest pagina, CancellationToken ct)
    {
        var consulta = db.Auditoria.Where(r => r.Entidad == entidad && r.EntidadId == entidadId);

        var total = await consulta.CountAsync(ct);
        var items = await consulta
            .OrderByDescending(r => r.Fecha).ThenByDescending(r => r.Id)
            .Skip(pagina.Skip).Take(pagina.PageSize)
            .ToListAsync(ct);

        return new PagedResult<RegistroAuditoria>(items, pagina.Page, pagina.PageSize, total);
    }
}

internal sealed class DirectorioDeUsuarios(PuntoIosDbContext db) : IDirectorioDeUsuarios
{
    public async Task<IReadOnlyDictionary<Guid, string>> ObtenerNombresAsync(
        IEnumerable<Guid> usuarioIds, CancellationToken ct)
    {
        var ids = usuarioIds.Distinct().ToList();
        if (ids.Count == 0)
        {
            return new Dictionary<Guid, string>();
        }

        return await db.Usuarios
            .Where(u => ids.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, u => u.Nombre, ct);
    }
}
