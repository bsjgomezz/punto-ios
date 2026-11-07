using Microsoft.EntityFrameworkCore;
using PuntoIOS.Application.Features.Dispositivos.Puertos;
using PuntoIOS.Domain.Clientes;

namespace PuntoIOS.Infrastructure.Persistence;

internal sealed class DispositivoRepositorio(PuntoIosDbContext db) : IDispositivoRepositorio
{
    public Task<Dispositivo?> ObtenerPorIdAsync(Guid id, CancellationToken ct) =>
        db.Dispositivos.FirstOrDefaultAsync(d => d.Id == id, ct);

    public async Task<IReadOnlyList<Dispositivo>> ListarPorClienteAsync(Guid clienteId, CancellationToken ct) =>
        await db.Dispositivos
            .Where(d => d.ClienteId == clienteId)
            .OrderByDescending(d => d.CreadoEn).ThenByDescending(d => d.Id)
            .ToListAsync(ct);

    public Task<Dispositivo?> ObtenerPorImeiAsync(string imeiNormalizado, CancellationToken ct) =>
        db.Dispositivos.FirstOrDefaultAsync(d => d.ImeiSerie == imeiNormalizado, ct);

    public void Agregar(Dispositivo dispositivo) => db.Dispositivos.Add(dispositivo);

    public Task GuardarCambiosAsync(CancellationToken ct) => db.SaveChangesAsync(ct);
}
