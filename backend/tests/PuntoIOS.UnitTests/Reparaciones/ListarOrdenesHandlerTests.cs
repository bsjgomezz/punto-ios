using PuntoIOS.Application.Common.Pagination;
using PuntoIOS.Application.Features.Reparaciones.ConsultarOrdenes;
using PuntoIOS.Application.Features.Reparaciones.Puertos;
using PuntoIOS.Domain.Clientes;
using PuntoIOS.Domain.Reparaciones;
using PuntoIOS.UnitTests.Clientes;

namespace PuntoIOS.UnitTests.Reparaciones;

public class ListarOrdenesHandlerTests
{
    private static readonly DateTimeOffset Ahora = new(2026, 11, 8, 12, 0, 0, TimeSpan.Zero);
    private static readonly Guid LocalId = Guid.NewGuid();
    private static readonly Guid TecnicoA = Guid.NewGuid();
    private static readonly Guid TecnicoB = Guid.NewGuid();

    private readonly ClienteRepoEnMemoria _clientes = new();
    private readonly DispositivoRepoEnMemoria _dispositivos = new();
    private readonly OrdenRepoEnMemoria _ordenes = new();

    private readonly OrdenReparacion _activaDeA;
    private readonly OrdenReparacion _entregadaDeA;
    private readonly OrdenReparacion _canceladaDeA;
    private readonly OrdenReparacion _activaDeB;
    private readonly OrdenReparacion _activaSinTecnico;

    public ListarOrdenesHandlerTests()
    {
        var ana = Cliente.Registrar(LocalId, "Ana", "Pérez", "3498 111111", null, null, null, null, Ahora);
        _clientes.Clientes.Add(ana);
        var iphone = Dispositivo.Registrar(LocalId, ana.Id, "Apple", "iPhone 13", null, null, null, null, null, Ahora);
        _dispositivos.Dispositivos.Add(iphone);

        OrdenReparacion Nueva(Guid? tecnico)
        {
            var orden = OrdenReparacion.Recibir(
                LocalId, ana.Id, iphone.Id, "No carga", [TiposDeReparacion.ConectorDeCarga], null, tecnico, null,
                null, null, null, Ahora);
            _ordenes.Ordenes.Add(orden);
            return orden;
        }

        _activaDeA = Nueva(TecnicoA);
        _entregadaDeA = Nueva(TecnicoA);
        foreach (var estado in new[]
        {
            EstadosDeReparacion.EnPruebas, EstadosDeReparacion.EnDiagnostico,
            EstadosDeReparacion.Irreparable, EstadosDeReparacion.Entregado,
        })
        {
            _entregadaDeA.CambiarEstado(estado);
        }

        _canceladaDeA = Nueva(TecnicoA);
        _canceladaDeA.CambiarEstado(EstadosDeReparacion.Cancelado);
        _activaDeB = Nueva(TecnicoB);
        _activaDeB.CambiarEstado(EstadosDeReparacion.EnPruebas);
        _activaSinTecnico = Nueva(null);
    }

    private static List<Guid> Ordenadas(params OrdenReparacion[] ordenes) => ordenes.Select(o => o.Id).Order().ToList();

    private async Task<List<Guid>> ListarAsync(FiltroDeOrdenes filtro)
    {
        var handler = new ListarOrdenesHandler(
            _ordenes, _clientes, _dispositivos, new DirectorioFijo(new Dictionary<Guid, string> { [TecnicoA] = "A", [TecnicoB] = "B" }));
        var resultado = await handler.HandleAsync(filtro, new PageRequest(1, PageRequest.MaxPageSize), default);
        return resultado.Items.Select(o => o.Id).Order().ToList();
    }

    [Fact]
    public async Task Sin_filtro_devuelve_todas_incluidas_las_entregadas_y_canceladas()
    {
        var ids = await ListarAsync(new FiltroDeOrdenes());

        Assert.Equal(Ordenadas(_activaDeA, _entregadaDeA, _canceladaDeA, _activaDeB, _activaSinTecnico), ids);
    }

    [Fact]
    public async Task Por_tecnico_devuelve_solo_las_asignadas_a_ese_tecnico()
    {
        var ids = await ListarAsync(new FiltroDeOrdenes(TecnicoId: TecnicoB));

        Assert.Equal(Ordenadas(_activaDeB), ids);
    }

    [Fact]
    public async Task Solo_activas_excluye_entregadas_y_canceladas()
    {
        var ids = await ListarAsync(new FiltroDeOrdenes(SoloActivas: true));

        Assert.Equal(Ordenadas(_activaDeA, _activaDeB, _activaSinTecnico), ids);
    }

    [Fact]
    public async Task Por_tecnico_y_solo_activas_combina_ambos_filtros()
    {
        var ids = await ListarAsync(new FiltroDeOrdenes(TecnicoId: TecnicoA, SoloActivas: true));

        Assert.Equal(Ordenadas(_activaDeA), ids);
    }
}
