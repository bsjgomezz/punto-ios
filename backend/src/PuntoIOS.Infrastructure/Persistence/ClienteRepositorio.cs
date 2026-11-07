using Microsoft.EntityFrameworkCore;
using PuntoIOS.Application.Common.Pagination;
using PuntoIOS.Application.Features.Clientes.Puertos;
using PuntoIOS.Domain.Clientes;

namespace PuntoIOS.Infrastructure.Persistence;

internal sealed class ClienteRepositorio(PuntoIosDbContext db) : IClienteRepositorio
{
    private const int MinimoDigitosParaBuscarTelefono = 3;

    public Task<Cliente?> ObtenerPorIdAsync(Guid id, CancellationToken ct) =>
        db.Clientes.FirstOrDefaultAsync(c => c.Id == id, ct);

    public async Task<IReadOnlyList<Cliente>> ObtenerPorIdsAsync(IEnumerable<Guid> ids, CancellationToken ct)
    {
        var lista = ids.ToList();
        return await db.Clientes.Where(c => lista.Contains(c.Id)).ToListAsync(ct);
    }

    public async Task<IReadOnlyList<Cliente>> BuscarPorTelefonoAsync(string telefonoNormalizado, CancellationToken ct) =>
        await db.Clientes
            .Where(c => c.TelefonoNormalizado == telefonoNormalizado)
            .OrderBy(c => c.Apellido).ThenBy(c => c.Nombre)
            .Take(5)
            .ToListAsync(ct);

    public async Task<PagedResult<Cliente>> BuscarAsync(string? texto, PageRequest pagina, CancellationToken ct)
    {
        IQueryable<Cliente> consulta = db.Clientes;

        var busqueda = texto ?? string.Empty;
        var esBusquedaDeTelefono = busqueda.All(c => !char.IsLetter(c))
            && Cliente.ContarDigitos(busqueda) >= MinimoDigitosParaBuscarTelefono;

        if (esBusquedaDeTelefono)
        {
            // Solo números y símbolos ("+54 9 3498-12-3456"): se compara contra el teléfono normalizado.
            var digitos = Cliente.NormalizarTelefono(busqueda);
            consulta = consulta.Where(c => c.TelefonoNormalizado.Contains(digitos));
        }
        else
        {
            // Cada palabra debe aparecer en el nombre, apellido o email (todas las palabras juntas).
            foreach (var palabra in busqueda.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries))
            {
                var patron = $"%{EscaparLike(palabra)}%";
                consulta = consulta.Where(c =>
                    EF.Functions.ILike(c.Nombre, patron)
                    || EF.Functions.ILike(c.Apellido, patron)
                    || (c.Email != null && EF.Functions.ILike(c.Email, patron)));
            }
        }

        var total = await consulta.CountAsync(ct);
        var items = await consulta
            .OrderBy(c => c.Apellido).ThenBy(c => c.Nombre).ThenBy(c => c.Id)
            .Skip(pagina.Skip).Take(pagina.PageSize)
            .ToListAsync(ct);

        return new PagedResult<Cliente>(items, pagina.Page, pagina.PageSize, total);
    }

    public void Agregar(Cliente cliente) => db.Clientes.Add(cliente);

    public Task GuardarCambiosAsync(CancellationToken ct) => db.SaveChangesAsync(ct);

    private static string EscaparLike(string valor) =>
        valor.Replace("\\", "\\\\").Replace("%", "\\%").Replace("_", "\\_");
}
