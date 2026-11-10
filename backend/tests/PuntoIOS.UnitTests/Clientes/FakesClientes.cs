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

    public Task<IReadOnlyList<Cliente>> ObtenerPorIdsAsync(IEnumerable<Guid> ids, CancellationToken ct) =>
        Task.FromResult<IReadOnlyList<Cliente>>(Clientes.Where(c => ids.Contains(c.Id)).ToList());

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

internal sealed class DirectorioFijo(Dictionary<Guid, string> nombres, IReadOnlyList<UsuarioResumen>? conPermiso = null)
    : IDirectorioDeUsuarios
{
    public Task<IReadOnlyDictionary<Guid, string>> ObtenerNombresAsync(IEnumerable<Guid> usuarioIds, CancellationToken ct) =>
        Task.FromResult<IReadOnlyDictionary<Guid, string>>(nombres);

    public Task<IReadOnlyList<UsuarioResumen>> ListarActivosConPermisoAsync(string permiso, CancellationToken ct) =>
        Task.FromResult(conPermiso ?? []);
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

    public Task<IReadOnlyList<Dispositivo>> ObtenerPorIdsAsync(IEnumerable<Guid> ids, CancellationToken ct) =>
        Task.FromResult<IReadOnlyList<Dispositivo>>(Dispositivos.Where(d => ids.Contains(d.Id)).ToList());

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

internal sealed class OrdenRepoEnMemoria : PuntoIOS.Application.Features.Reparaciones.Puertos.IOrdenReparacionRepositorio
{
    public List<PuntoIOS.Domain.Reparaciones.OrdenReparacion> Ordenes { get; } = [];

    public int Guardados { get; private set; }

    public Task<PuntoIOS.Domain.Reparaciones.OrdenReparacion?> ObtenerPorIdAsync(Guid id, CancellationToken ct) =>
        Task.FromResult(Ordenes.FirstOrDefault(o => o.Id == id));

    public Task<PagedResult<PuntoIOS.Domain.Reparaciones.OrdenReparacion>> ListarAsync(
        PuntoIOS.Application.Features.Reparaciones.Puertos.FiltroDeOrdenes filtro, PageRequest pagina, CancellationToken ct)
    {
        var items = Ordenes
            .Where(o => filtro.ClienteId is null || o.ClienteId == filtro.ClienteId)
            .Where(o => filtro.TecnicoId is null || o.TecnicoUsuarioId == filtro.TecnicoId)
            .Where(o => !filtro.SoloActivas
                || (o.Estado != PuntoIOS.Domain.Reparaciones.EstadosDeReparacion.Entregado
                    && o.Estado != PuntoIOS.Domain.Reparaciones.EstadosDeReparacion.Cancelado))
            .ToList();
        return Task.FromResult(new PagedResult<PuntoIOS.Domain.Reparaciones.OrdenReparacion>(items, pagina.Page, pagina.PageSize, items.Count));
    }

    public void Agregar(PuntoIOS.Domain.Reparaciones.OrdenReparacion orden) => Ordenes.Add(orden);

    public List<PuntoIOS.Domain.Reparaciones.FotoOrden> Fotos { get; } = [];

    public void AgregarFoto(PuntoIOS.Domain.Reparaciones.FotoOrden foto) => Fotos.Add(foto);

    public Task<IReadOnlyList<PuntoIOS.Domain.Reparaciones.FotoOrden>> ListarFotosAsync(Guid ordenId, CancellationToken ct) =>
        Task.FromResult<IReadOnlyList<PuntoIOS.Domain.Reparaciones.FotoOrden>>(Fotos.Where(f => f.OrdenId == ordenId).ToList());

    public Task<PuntoIOS.Domain.Reparaciones.FotoOrden?> ObtenerFotoAsync(Guid ordenId, Guid fotoId, CancellationToken ct) =>
        Task.FromResult(Fotos.FirstOrDefault(f => f.OrdenId == ordenId && f.Id == fotoId));

    public List<PuntoIOS.Domain.Reparaciones.VersionDePresupuesto> VersionesPresupuesto { get; } = [];

    public void AgregarVersionPresupuesto(PuntoIOS.Domain.Reparaciones.VersionDePresupuesto version) =>
        VersionesPresupuesto.Add(version);

    public Task<IReadOnlyList<PuntoIOS.Domain.Reparaciones.VersionDePresupuesto>> ListarVersionesPresupuestoAsync(
        Guid ordenId, CancellationToken ct) =>
        Task.FromResult<IReadOnlyList<PuntoIOS.Domain.Reparaciones.VersionDePresupuesto>>(
            VersionesPresupuesto.Where(v => v.OrdenId == ordenId).OrderByDescending(v => v.Numero).ToList());

    public Task<PuntoIOS.Domain.Reparaciones.VersionDePresupuesto?> ObtenerVersionVigentePresupuestoAsync(
        Guid ordenId, CancellationToken ct) =>
        Task.FromResult(VersionesPresupuesto.Where(v => v.OrdenId == ordenId).OrderByDescending(v => v.Numero).FirstOrDefault());

    public Task<IReadOnlyList<PuntoIOS.Domain.Reparaciones.VersionDePresupuesto>> ObtenerVersionesVigentesPresupuestoAsync(
        IEnumerable<Guid> ordenIds, CancellationToken ct)
    {
        var ids = ordenIds.ToHashSet();
        return Task.FromResult<IReadOnlyList<PuntoIOS.Domain.Reparaciones.VersionDePresupuesto>>(
            VersionesPresupuesto
                .Where(v => ids.Contains(v.OrdenId))
                .GroupBy(v => v.OrdenId)
                .Select(g => g.OrderByDescending(v => v.Numero).First())
                .ToList());
    }

    public Task GuardarCambiosAsync(CancellationToken ct)
    {
        Guardados++;
        return Task.CompletedTask;
    }
}

internal sealed class AlmacenEnMemoria : PuntoIOS.Application.Common.Archivos.IAlmacenDeArchivos
{
    public Dictionary<string, byte[]> Archivos { get; } = [];

    public async Task GuardarAsync(string ruta, Stream contenido, CancellationToken ct)
    {
        using var copia = new MemoryStream();
        await contenido.CopyToAsync(copia, ct);
        Archivos[ruta] = copia.ToArray();
    }

    public Task<Stream?> AbrirAsync(string ruta, CancellationToken ct) =>
        Task.FromResult<Stream?>(Archivos.TryGetValue(ruta, out var bytes) ? new MemoryStream(bytes) : null);
}
