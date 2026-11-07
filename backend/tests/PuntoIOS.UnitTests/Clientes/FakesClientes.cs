using PuntoIOS.Application.Common.Multilocal;
using PuntoIOS.Application.Common.Pagination;
using PuntoIOS.Application.Common.Security;
using PuntoIOS.Application.Features.Auditoria.Puertos;
using PuntoIOS.Application.Features.Clientes.Puertos;
using PuntoIOS.Application.Features.Identidad.Puertos;
using PuntoIOS.Domain.Auditoria;
using PuntoIOS.Domain.Clientes;

namespace PuntoIOS.UnitTests.Clientes;

internal sealed class ClienteRepoEnMemoria : IClienteRepositorio
{
    public List<Cliente> Clientes { get; } = [];

    public int Guardados { get; private set; }

    public Task<Cliente?> ObtenerPorIdAsync(Guid id, CancellationToken ct) =>
        Task.FromResult(Clientes.FirstOrDefault(c => c.Id == id));

    public Task<IReadOnlyList<Cliente>> BuscarPorTelefonoAsync(string telefonoNormalizado, CancellationToken ct) =>
        Task.FromResult<IReadOnlyList<Cliente>>(Clientes.Where(c => c.TelefonoNormalizado == telefonoNormalizado).ToList());

    public Task<PagedResult<Cliente>> BuscarAsync(string? texto, PageRequest pagina, CancellationToken ct) =>
        Task.FromResult(new PagedResult<Cliente>(Clientes, pagina.Page, pagina.PageSize, Clientes.Count));

    public void Agregar(Cliente cliente) => Clientes.Add(cliente);

    public Task GuardarCambiosAsync(CancellationToken ct)
    {
        Guardados++;
        return Task.CompletedTask;
    }
}

internal sealed class AuditoriaRepoEnMemoria : IAuditoriaRepositorio
{
    public List<RegistroAuditoria> Registros { get; } = [];

    public void Agregar(RegistroAuditoria registro) => Registros.Add(registro);

    public Task<PagedResult<RegistroAuditoria>> ListarAsync(string entidad, Guid entidadId, PageRequest pagina, CancellationToken ct)
    {
        var items = Registros
            .Where(r => r.Entidad == entidad && r.EntidadId == entidadId)
            .OrderByDescending(r => r.Fecha)
            .ToList();
        return Task.FromResult(new PagedResult<RegistroAuditoria>(
            items.Skip(pagina.Skip).Take(pagina.PageSize).ToList(), pagina.Page, pagina.PageSize, items.Count));
    }
}

internal sealed class DirectorioFijo(Dictionary<Guid, string> nombres) : IDirectorioDeUsuarios
{
    public Task<IReadOnlyDictionary<Guid, string>> ObtenerNombresAsync(IEnumerable<Guid> usuarioIds, CancellationToken ct) =>
        Task.FromResult<IReadOnlyDictionary<Guid, string>>(nombres);
}

internal sealed class LocalFijo(Guid localId) : ILocalActual
{
    public Guid LocalId => localId;
}

internal sealed class UsuarioFijo(Guid usuarioId) : ICurrentUserAccessor
{
    public CurrentUser? GetCurrentUser() => new("uid", "a@b.com", true, null, usuarioId);
}

internal sealed class DispositivoRepoEnMemoria : PuntoIOS.Application.Features.Dispositivos.Puertos.IDispositivoRepositorio
{
    public List<Dispositivo> Dispositivos { get; } = [];

    public int Guardados { get; private set; }

    public Task<Dispositivo?> ObtenerPorIdAsync(Guid id, CancellationToken ct) =>
        Task.FromResult(Dispositivos.FirstOrDefault(d => d.Id == id));

    public Task<IReadOnlyList<Dispositivo>> ListarPorClienteAsync(Guid clienteId, CancellationToken ct) =>
        Task.FromResult<IReadOnlyList<Dispositivo>>(
            Dispositivos.Where(d => d.ClienteId == clienteId).OrderByDescending(d => d.CreadoEn).ToList());

    public Task<Dispositivo?> ObtenerPorImeiAsync(string imeiNormalizado, CancellationToken ct) =>
        Task.FromResult(Dispositivos.FirstOrDefault(d => d.ImeiSerie == imeiNormalizado));

    public void Agregar(Dispositivo dispositivo) => Dispositivos.Add(dispositivo);

    public Task GuardarCambiosAsync(CancellationToken ct)
    {
        Guardados++;
        return Task.CompletedTask;
    }
}
