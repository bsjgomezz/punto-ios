using PuntoIOS.Application.Features.Reparaciones.CambiarEstado;
using PuntoIOS.Domain.Auditoria;
using PuntoIOS.Domain.Clientes;
using PuntoIOS.Domain.Reparaciones;
using PuntoIOS.UnitTests.Clientes;
using PuntoIOS.UnitTests.Identidad;

namespace PuntoIOS.UnitTests.Reparaciones;

public class CambiarEstadoOrdenHandlerTests
{
    private static readonly DateTimeOffset Ahora = new(2026, 11, 8, 12, 0, 0, TimeSpan.Zero);
    private static readonly Guid LocalId = Guid.NewGuid();
    private static readonly Guid UsuarioId = Guid.NewGuid();

    private static readonly string[] HastaPresupuestoEnviado =
    [
        EstadosDeReparacion.EnPruebas, EstadosDeReparacion.EnDiagnostico,
        EstadosDeReparacion.EsperandoPresupuesto, EstadosDeReparacion.PresupuestoEnviado,
    ];

    private readonly ClienteRepoEnMemoria _clientes = new();
    private readonly DispositivoRepoEnMemoria _dispositivos = new();
    private readonly AuditoriaRepoEnMemoria _auditoria = new();
    private readonly OrdenRepoEnMemoria _ordenes = new();
    private readonly OrdenReparacion _orden;

    public CambiarEstadoOrdenHandlerTests()
    {
        var ana = Cliente.Registrar(LocalId, "Ana", "Pérez", "3498 111111", null, null, null, null, Ahora);
        _clientes.Clientes.Add(ana);
        var iphone = Dispositivo.Registrar(LocalId, ana.Id, "Apple", "iPhone 13", null, null, null, null, null, Ahora);
        _dispositivos.Dispositivos.Add(iphone);
        _orden = OrdenReparacion.Recibir(
            LocalId, ana.Id, iphone.Id, "No carga", [TiposDeReparacion.ConectorDeCarga], null, null, null,
            TiposDeDesbloqueo.Pin, "cifrado", null, Ahora);
        _ordenes.Ordenes.Add(_orden);
    }

    private CambiarEstadoOrdenHandler Crear() =>
        new(_ordenes, _clientes, _dispositivos, _auditoria, new DirectorioFijo([]),
            new LocalFijo(LocalId), new UsuarioFijo(UsuarioId), new RelojFijo(Ahora));

    /// <summary>Mueve la orden directo en el dominio (sin pasar por el handler ni dejar auditoría).</summary>
    private void Llevar(params string[] estados)
    {
        foreach (var estado in estados)
        {
            _orden.CambiarEstado(estado);
        }
    }

    [Fact]
    public async Task Una_orden_inexistente_devuelve_no_encontrado()
    {
        var resultado = await Crear().HandleAsync(new CambiarEstadoOrdenCommand(Guid.NewGuid(), EstadosDeReparacion.EnPruebas), default);

        Assert.Equal("orden_inexistente", resultado.Error!.Codigo);
        Assert.Empty(_auditoria.Registros);
    }

    [Theory]
    [InlineData("teletransportado")]
    [InlineData(" ")]
    [InlineData((string?)null)]
    public async Task Un_estado_que_no_existe_se_rechaza(string? estado)
    {
        var resultado = await Crear().HandleAsync(new CambiarEstadoOrdenCommand(_orden.Id, estado), default);

        Assert.Equal("estado_invalido", resultado.Error!.Codigo);
        Assert.Equal(EstadosDeReparacion.Recibido, _orden.Estado);
        Assert.Empty(_auditoria.Registros);
    }

    [Theory]
    [InlineData("recibido")]
    public async Task Una_transicion_no_permitida_desde_el_estado_actual_se_rechaza(string estado)
    {
        var resultado = await Crear().HandleAsync(new CambiarEstadoOrdenCommand(_orden.Id, estado), default);

        Assert.Equal("transicion_invalida", resultado.Error!.Codigo);
        Assert.Equal(EstadosDeReparacion.Recibido, _orden.Estado);
        Assert.Empty(_auditoria.Registros);
        Assert.Equal(0, _ordenes.Guardados);
    }

    [Theory]
    [InlineData(EstadosDeReparacion.Aprobado)]
    [InlineData(EstadosDeReparacion.PresupuestoRechazado)]
    public async Task Aprobar_o_rechazar_el_presupuesto_no_se_hace_con_el_cambio_generico_aunque_la_tabla_lo_permita(string estado)
    {
        Llevar([.. HastaPresupuestoEnviado, EstadosDeReparacion.EsperandoAprobacion]);
        Assert.True(MaquinaDeEstados.PuedeTransicionar(_orden.Estado, estado));

        var resultado = await Crear().HandleAsync(new CambiarEstadoOrdenCommand(_orden.Id, estado), default);

        Assert.Equal("usar_respuesta_presupuesto", resultado.Error!.Codigo);
        Assert.Equal(EstadosDeReparacion.EsperandoAprobacion, _orden.Estado);
        Assert.Empty(_auditoria.Registros);
        Assert.Equal(0, _ordenes.Guardados);
    }

    [Fact]
    public async Task Entregar_no_se_hace_con_el_cambio_generico_aunque_la_tabla_no_lo_permita()
    {
        // Desde "recibido" la tabla ni siquiera lo permitiría, pero el chequeo de "usar_entrega" va primero.
        Assert.False(MaquinaDeEstados.PuedeTransicionar(_orden.Estado, EstadosDeReparacion.Entregado));

        var resultado = await Crear().HandleAsync(new CambiarEstadoOrdenCommand(_orden.Id, EstadosDeReparacion.Entregado), default);

        Assert.Equal("usar_entrega", resultado.Error!.Codigo);
        Assert.Equal(EstadosDeReparacion.Recibido, _orden.Estado);
        Assert.Empty(_auditoria.Registros);
        Assert.Equal(0, _ordenes.Guardados);
    }

    [Fact]
    public async Task Entregar_no_se_hace_con_el_cambio_generico_aunque_la_tabla_lo_permita()
    {
        Llevar(
            EstadosDeReparacion.EnPruebas, EstadosDeReparacion.EnDiagnostico, EstadosDeReparacion.EsperandoPresupuesto,
            EstadosDeReparacion.PresupuestoEnviado, EstadosDeReparacion.EsperandoAprobacion, EstadosDeReparacion.Aprobado,
            EstadosDeReparacion.EnReparacion, EstadosDeReparacion.ReparacionCompletada, EstadosDeReparacion.ListoParaRetirar);
        Assert.True(MaquinaDeEstados.PuedeTransicionar(_orden.Estado, EstadosDeReparacion.Entregado));

        var resultado = await Crear().HandleAsync(new CambiarEstadoOrdenCommand(_orden.Id, EstadosDeReparacion.Entregado), default);

        Assert.Equal("usar_entrega", resultado.Error!.Codigo);
        Assert.Equal(EstadosDeReparacion.ListoParaRetirar, _orden.Estado);
        Assert.True(_orden.TieneDesbloqueo);
        Assert.Empty(_auditoria.Registros);
        Assert.Equal(0, _ordenes.Guardados);
    }

    [Fact]
    public async Task Listo_para_retirar_no_ofrece_entregado_como_estado_disponible()
    {
        Llevar(
            EstadosDeReparacion.EnPruebas, EstadosDeReparacion.EnDiagnostico, EstadosDeReparacion.EsperandoPresupuesto,
            EstadosDeReparacion.PresupuestoEnviado, EstadosDeReparacion.EsperandoAprobacion, EstadosDeReparacion.Aprobado,
            EstadosDeReparacion.EnReparacion);

        var resultado = await Crear().HandleAsync(
            new CambiarEstadoOrdenCommand(_orden.Id, EstadosDeReparacion.ReparacionCompletada), default);
        Assert.True(resultado.EsExitoso);
        resultado = await Crear().HandleAsync(new CambiarEstadoOrdenCommand(_orden.Id, EstadosDeReparacion.ListoParaRetirar), default);

        // En la tabla "entregado" es el único destino, pero solo se llega con el endpoint de entrega.
        Assert.True(resultado.EsExitoso);
        Assert.Empty(resultado.Valor!.EstadosDisponibles);
    }

    [Fact]
    public async Task Esperando_aprobacion_no_ofrece_aprobado_ni_presupuesto_rechazado_como_estados_disponibles()
    {
        Llevar(HastaPresupuestoEnviado);

        var resultado = await Crear().HandleAsync(new CambiarEstadoOrdenCommand(_orden.Id, EstadosDeReparacion.EsperandoAprobacion), default);

        // En la tabla son los únicos dos destinos, pero solo se llega a ellos respondiendo el presupuesto.
        Assert.True(resultado.EsExitoso);
        Assert.Equal(EstadosDeReparacion.EsperandoAprobacion, resultado.Valor!.Estado);
        Assert.DoesNotContain(EstadosDeReparacion.Aprobado, resultado.Valor.EstadosDisponibles);
        Assert.DoesNotContain(EstadosDeReparacion.PresupuestoRechazado, resultado.Valor.EstadosDisponibles);
        Assert.Empty(resultado.Valor.EstadosDisponibles);
    }

    [Fact]
    public async Task Una_transicion_valida_cambia_el_estado_y_deja_registro_del_anterior_y_el_nuevo()
    {
        var resultado = await Crear().HandleAsync(new CambiarEstadoOrdenCommand(_orden.Id, EstadosDeReparacion.EnPruebas), default);

        Assert.True(resultado.EsExitoso);
        Assert.Equal(EstadosDeReparacion.EnPruebas, resultado.Valor!.Estado);
        Assert.Equal([EstadosDeReparacion.EnDiagnostico, EstadosDeReparacion.Cancelado], resultado.Valor.EstadosDisponibles);
        Assert.Equal(1, _ordenes.Guardados);

        var registro = Assert.Single(_auditoria.Registros);
        Assert.Equal(AccionesAuditadas.EstadoCambiado, registro.Accion);
        Assert.Equal(EntidadesAuditadas.Reparacion, registro.Entidad);
        Assert.Equal((_orden.Id, UsuarioId, Ahora), (registro.EntidadId, registro.UsuarioId, registro.Fecha));
        var cambio = Assert.Single(registro.Cambios);
        Assert.Equal(new CambioDeCampo("estado", EstadosDeReparacion.Recibido, EstadosDeReparacion.EnPruebas), cambio);
    }

    [Fact]
    public async Task Al_cancelar_la_orden_queda_sin_estados_disponibles()
    {
        var resultado = await Crear().HandleAsync(new CambiarEstadoOrdenCommand(_orden.Id, EstadosDeReparacion.Cancelado), default);

        Assert.True(resultado.EsExitoso);
        Assert.Empty(resultado.Valor!.EstadosDisponibles);
    }
}
