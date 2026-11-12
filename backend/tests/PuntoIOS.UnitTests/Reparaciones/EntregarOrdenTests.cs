using System.Globalization;
using PuntoIOS.Application.Common.Results;
using PuntoIOS.Application.Features.Reparaciones.EntregarOrden;
using PuntoIOS.Domain.Auditoria;
using PuntoIOS.Domain.Clientes;
using PuntoIOS.Domain.Reparaciones;
using PuntoIOS.UnitTests.Clientes;
using PuntoIOS.UnitTests.Identidad;

namespace PuntoIOS.UnitTests.Reparaciones;

public class RegistrarEntregaTests
{
    private static readonly DateTimeOffset Ahora = new(2026, 11, 12, 15, 30, 0, TimeSpan.Zero);

    private static OrdenReparacion Orden() =>
        OrdenReparacion.Recibir(
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "No carga", [TiposDeReparacion.ConectorDeCarga], null, null, null,
            null, null, null, Ahora);

    private static void AssertSinEntrega(OrdenReparacion orden)
    {
        Assert.Null(orden.GarantiaCodigo);
        Assert.Null(orden.GarantiaDias);
        Assert.Null(orden.GarantiaVenceEn);
        Assert.Null(orden.MontoCobrado);
        Assert.Null(orden.MetodoPago);
        Assert.Null(orden.EntregadoEn);
    }

    [Fact]
    public void Una_orden_nueva_no_tiene_entrega_registrada()
    {
        AssertSinEntrega(Orden());
    }

    [Theory]
    [InlineData("90_dias")]
    [InlineData("")]
    [InlineData((string?)null)]
    public void Una_garantia_inexistente_se_rechaza_sin_cambiar_nada(string? garantia)
    {
        var orden = Orden();

        var ex = Assert.Throws<ArgumentException>(() => orden.RegistrarEntrega(garantia, 1000m, MetodosDePago.Efectivo, Ahora));

        Assert.Equal("garantiaCodigo", ex.ParamName);
        AssertSinEntrega(orden);
        Assert.Equal(EstadosDeReparacion.Recibido, orden.Estado);
    }

    [Theory]
    [InlineData("cheque")]
    [InlineData((string?)null)]
    public void Un_metodo_de_pago_inexistente_se_rechaza_sin_cambiar_nada(string? metodo)
    {
        var orden = Orden();

        var ex = Assert.Throws<ArgumentException>(() => orden.RegistrarEntrega(OpcionesDeGarantia.Dias30, 1000m, metodo, Ahora));

        Assert.Equal("metodoPago", ex.ParamName);
        AssertSinEntrega(orden);
    }

    [Theory]
    [InlineData("-0.01")]
    [InlineData("10.001")]
    [InlineData("100000000")]
    public void Un_monto_invalido_se_rechaza_sin_cambiar_nada(string monto)
    {
        var orden = Orden();
        var valor = decimal.Parse(monto, CultureInfo.InvariantCulture);

        var ex = Assert.Throws<ArgumentException>(
            () => orden.RegistrarEntrega(OpcionesDeGarantia.Dias30, valor, MetodosDePago.Efectivo, Ahora));

        Assert.Equal("montoCobrado", ex.ParamName);
        AssertSinEntrega(orden);
    }

    [Fact]
    public void Un_monto_mayor_al_maximo_se_rechaza()
    {
        var orden = Orden();

        Assert.Throws<ArgumentException>(
            () => orden.RegistrarEntrega(
                OpcionesDeGarantia.SinGarantia, OrdenReparacion.MontoCobradoMaximo + 0.01m, MetodosDePago.Efectivo, Ahora));
        AssertSinEntrega(orden);
    }

    [Fact]
    public void El_monto_maximo_es_valido()
    {
        var orden = Orden();

        orden.RegistrarEntrega(OpcionesDeGarantia.SinGarantia, OrdenReparacion.MontoCobradoMaximo, MetodosDePago.Efectivo, Ahora);

        Assert.Equal(OrdenReparacion.MontoCobradoMaximo, orden.MontoCobrado);
    }

    [Fact]
    public void Un_monto_cero_es_valido()
    {
        // Puede haber entregas sin cobro (por ejemplo, un equipo que vuelve por garantía).
        var orden = Orden();

        orden.RegistrarEntrega(OpcionesDeGarantia.SinGarantia, 0m, MetodosDePago.Efectivo, Ahora);

        Assert.Equal(0m, orden.MontoCobrado);
    }

    [Theory]
    [InlineData("sin_garantia", null, null)]
    [InlineData("15_dias", 15, "2026-11-27")]
    [InlineData("30_dias", 30, "2026-12-12")]
    [InlineData("45_dias", 45, "2026-12-27")]
    [InlineData("60_dias", 60, "2027-01-11")]
    public void Cada_opcion_de_garantia_graba_sus_dias_y_su_vencimiento(string garantia, int? dias, string? vence)
    {
        var orden = Orden();

        orden.RegistrarEntrega(garantia, 25000.50m, MetodosDePago.TransferenciaBancaria, Ahora);

        Assert.Equal(garantia, orden.GarantiaCodigo);
        Assert.Equal(dias, orden.GarantiaDias);
        Assert.Equal(vence is null ? (DateOnly?)null : DateOnly.Parse(vence, CultureInfo.InvariantCulture), orden.GarantiaVenceEn);
        Assert.Equal(25000.50m, orden.MontoCobrado);
        Assert.Equal(MetodosDePago.TransferenciaBancaria, orden.MetodoPago);
        Assert.Equal(Ahora, orden.EntregadoEn);
    }

    [Fact]
    public void Registrar_la_entrega_no_cambia_el_estado()
    {
        var orden = Orden();

        orden.RegistrarEntrega(OpcionesDeGarantia.Dias30, 1000m, MetodosDePago.Efectivo, Ahora);

        Assert.Equal(EstadosDeReparacion.Recibido, orden.Estado);
    }

    [Fact]
    public void Los_catalogos_de_garantia_tienen_las_mismas_opciones()
    {
        Assert.Equal(OpcionesDeGarantia.Catalogo.Keys.Order(), OpcionesDeGarantia.Duraciones.Keys.Order());
    }
}

public class EntregarOrdenHandlerTests
{
    private static readonly DateTimeOffset Ahora = new(2026, 11, 12, 15, 30, 0, TimeSpan.Zero);
    private static readonly Guid LocalId = Guid.NewGuid();
    private static readonly Guid UsuarioId = Guid.NewGuid();

    private static readonly string[] HastaListoParaRetirar =
    [
        EstadosDeReparacion.EnPruebas, EstadosDeReparacion.EnDiagnostico, EstadosDeReparacion.EsperandoPresupuesto,
        EstadosDeReparacion.PresupuestoEnviado, EstadosDeReparacion.EsperandoAprobacion, EstadosDeReparacion.Aprobado,
        EstadosDeReparacion.EnReparacion, EstadosDeReparacion.ReparacionCompletada, EstadosDeReparacion.ListoParaRetirar,
    ];

    private static readonly string[] HastaPresupuestoRechazado =
    [
        EstadosDeReparacion.EnPruebas, EstadosDeReparacion.EnDiagnostico, EstadosDeReparacion.EsperandoPresupuesto,
        EstadosDeReparacion.PresupuestoEnviado, EstadosDeReparacion.EsperandoAprobacion, EstadosDeReparacion.PresupuestoRechazado,
    ];

    private static readonly string[] HastaIrreparable =
    [
        EstadosDeReparacion.EnPruebas, EstadosDeReparacion.EnDiagnostico, EstadosDeReparacion.Irreparable,
    ];

    private readonly ClienteRepoEnMemoria _clientes = new();
    private readonly DispositivoRepoEnMemoria _dispositivos = new();
    private readonly AuditoriaRepoEnMemoria _auditoria = new();
    private readonly OrdenRepoEnMemoria _ordenes = new();
    private readonly OrdenReparacion _orden;

    public EntregarOrdenHandlerTests()
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

    private EntregarOrdenHandler Crear() =>
        new(_ordenes, _clientes, _dispositivos, _auditoria, new DirectorioFijo(new Dictionary<Guid, string> { [UsuarioId] = "Juan" }),
            new LocalFijo(LocalId), new UsuarioFijo(UsuarioId), new RelojFijo(Ahora));

    /// <summary>Mueve la orden directo en el dominio (sin pasar por ningún handler ni dejar auditoría).</summary>
    private void Llevar(params string[] estados)
    {
        foreach (var estado in estados)
        {
            _orden.CambiarEstado(estado);
        }
    }

    private void AssertSinCambios(string estado)
    {
        Assert.Equal(estado, _orden.Estado);
        Assert.Null(_orden.EntregadoEn);
        Assert.Null(_orden.GarantiaCodigo);
        Assert.Null(_orden.MontoCobrado);
        Assert.Null(_orden.MetodoPago);
        Assert.True(_orden.TieneDesbloqueo);
        Assert.Empty(_auditoria.Registros);
        Assert.Equal(0, _ordenes.Guardados);
    }

    [Fact]
    public async Task Una_orden_inexistente_devuelve_no_encontrado()
    {
        var resultado = await Crear().HandleAsync(
            new EntregarOrdenCommand(Guid.NewGuid(), OpcionesDeGarantia.Dias30, 1000m, MetodosDePago.Efectivo), default);

        Assert.Equal(TipoError.NoEncontrado, resultado.Error!.Tipo);
        Assert.Equal("orden_inexistente", resultado.Error.Codigo);
        Assert.Empty(_auditoria.Registros);
        Assert.Equal(0, _ordenes.Guardados);
    }

    [Theory]
    [InlineData(EstadosDeReparacion.Recibido)]
    [InlineData(EstadosDeReparacion.EnReparacion)]
    [InlineData(EstadosDeReparacion.EsperandoAprobacion)]
    public async Task Una_orden_en_un_estado_no_entregable_devuelve_conflicto(string estado)
    {
        var camino = new[]
        {
            EstadosDeReparacion.EnPruebas, EstadosDeReparacion.EnDiagnostico, EstadosDeReparacion.EsperandoPresupuesto,
            EstadosDeReparacion.PresupuestoEnviado, EstadosDeReparacion.EsperandoAprobacion, EstadosDeReparacion.Aprobado,
            EstadosDeReparacion.EnReparacion,
        };
        if (estado != EstadosDeReparacion.Recibido)
        {
            Llevar(camino[..(Array.IndexOf(camino, estado) + 1)]);
        }

        var resultado = await Crear().HandleAsync(
            new EntregarOrdenCommand(_orden.Id, OpcionesDeGarantia.Dias30, 1000m, MetodosDePago.Efectivo), default);

        Assert.Equal(TipoError.Conflicto, resultado.Error!.Tipo);
        Assert.Equal("fuera_de_estado_entregable", resultado.Error.Codigo);
        AssertSinCambios(estado);
    }

    [Fact]
    public async Task Una_orden_ya_entregada_devuelve_conflicto()
    {
        Llevar(HastaIrreparable);
        await Crear().HandleAsync(new EntregarOrdenCommand(_orden.Id, OpcionesDeGarantia.SinGarantia, 0m, MetodosDePago.Efectivo), default);
        var guardados = _ordenes.Guardados;
        var registros = _auditoria.Registros.Count;

        var resultado = await Crear().HandleAsync(
            new EntregarOrdenCommand(_orden.Id, OpcionesDeGarantia.Dias60, 5000m, MetodosDePago.TarjetaCredito), default);

        Assert.Equal("fuera_de_estado_entregable", resultado.Error!.Codigo);
        Assert.Equal(OpcionesDeGarantia.SinGarantia, _orden.GarantiaCodigo);
        Assert.Equal(0m, _orden.MontoCobrado);
        Assert.Equal(guardados, _ordenes.Guardados);
        Assert.Equal(registros, _auditoria.Registros.Count);
    }

    public static TheoryData<string> OrigenesEntregables => new()
    {
        EstadosDeReparacion.ListoParaRetirar, EstadosDeReparacion.PresupuestoRechazado, EstadosDeReparacion.Irreparable,
    };

    private void LlevarA(string origen) =>
        Llevar(origen switch
        {
            EstadosDeReparacion.ListoParaRetirar => HastaListoParaRetirar,
            EstadosDeReparacion.PresupuestoRechazado => HastaPresupuestoRechazado,
            _ => HastaIrreparable,
        });

    [Theory]
    [MemberData(nameof(OrigenesEntregables))]
    public async Task Entregar_registra_cobro_y_garantia_pasa_a_entregado_y_audita_ambas_cosas(string origen)
    {
        LlevarA(origen);
        Assert.Equal(origen, _orden.Estado);

        var resultado = await Crear().HandleAsync(
            new EntregarOrdenCommand(_orden.Id, OpcionesDeGarantia.Dias30, 25000.50m, MetodosDePago.TarjetaDebito), default);

        Assert.True(resultado.EsExitoso);
        Assert.Equal(EstadosDeReparacion.Entregado, _orden.Estado);
        Assert.False(_orden.TieneDesbloqueo);
        Assert.Equal(1, _ordenes.Guardados);

        var dto = resultado.Valor!;
        Assert.Equal(EstadosDeReparacion.Entregado, dto.Estado);
        Assert.Empty(dto.EstadosDisponibles);
        Assert.Equal(OpcionesDeGarantia.Dias30, dto.GarantiaCodigo);
        Assert.Equal(30, dto.GarantiaDias);
        Assert.Equal(new DateOnly(2026, 12, 12), dto.GarantiaVenceEn);
        Assert.Equal(25000.50m, dto.MontoCobrado);
        Assert.Equal(MetodosDePago.TarjetaDebito, dto.MetodoPago);
        Assert.Equal(Ahora, dto.EntregadoEn);
        Assert.Null(dto.DesbloqueoTipo);

        Assert.Equal(2, _auditoria.Registros.Count);
        var entrega = _auditoria.Registros[0];
        Assert.Equal(AccionesAuditadas.EntregaRegistrada, entrega.Accion);
        Assert.Equal(
            (EntidadesAuditadas.Reparacion, _orden.Id, UsuarioId, Ahora),
            (entrega.Entidad, entrega.EntidadId, entrega.UsuarioId, entrega.Fecha));
        Assert.Equal(
            [
                new CambioDeCampo("garantia", null, "30 días"),
                new CambioDeCampo("monto_cobrado", null, "25000.50"),
                new CambioDeCampo("metodo_pago", null, "Tarjeta de débito"),
            ],
            entrega.Cambios);

        var estado = _auditoria.Registros[1];
        Assert.Equal(AccionesAuditadas.EstadoCambiado, estado.Accion);
        Assert.Equal(
            (EntidadesAuditadas.Reparacion, _orden.Id, UsuarioId, Ahora),
            (estado.Entidad, estado.EntidadId, estado.UsuarioId, estado.Fecha));
        Assert.Equal([new CambioDeCampo("estado", origen, EstadosDeReparacion.Entregado)], estado.Cambios);
    }

    [Fact]
    public async Task Entregar_sin_garantia_deja_dias_y_vencimiento_en_null()
    {
        Llevar(HastaIrreparable);

        var resultado = await Crear().HandleAsync(
            new EntregarOrdenCommand(_orden.Id, OpcionesDeGarantia.SinGarantia, 0m, MetodosDePago.Efectivo), default);

        Assert.True(resultado.EsExitoso);
        Assert.Equal(OpcionesDeGarantia.SinGarantia, resultado.Valor!.GarantiaCodigo);
        Assert.Null(resultado.Valor.GarantiaDias);
        Assert.Null(resultado.Valor.GarantiaVenceEn);
        Assert.Equal(
            [
                new CambioDeCampo("garantia", null, "Sin garantía"),
                new CambioDeCampo("monto_cobrado", null, "0"),
                new CambioDeCampo("metodo_pago", null, "Efectivo"),
            ],
            _auditoria.Registros[0].Cambios);
    }

    [Theory]
    [InlineData("90_dias", 1000, "efectivo", "La garantía elegida no existe.")]
    [InlineData((string?)null, 1000, "efectivo", "La garantía elegida no existe.")]
    [InlineData("30_dias", 1000, "cheque", "El método de pago no existe.")]
    [InlineData("30_dias", -1, "efectivo", "El monto cobrado no es válido.")]
    public async Task Datos_de_entrega_invalidos_devuelven_validacion_sin_tocar_nada(
        string? garantia, int monto, string metodo, string mensaje)
    {
        Llevar(HastaListoParaRetirar);

        var resultado = await Crear().HandleAsync(new EntregarOrdenCommand(_orden.Id, garantia, monto, metodo), default);

        Assert.Equal(TipoError.Validacion, resultado.Error!.Tipo);
        Assert.Equal("entrega_invalida", resultado.Error.Codigo);
        Assert.Equal(mensaje, resultado.Error.Mensaje);
        AssertSinCambios(EstadosDeReparacion.ListoParaRetirar);
    }

    [Fact]
    public async Task Un_monto_con_mas_de_dos_decimales_devuelve_validacion()
    {
        Llevar(HastaListoParaRetirar);

        var resultado = await Crear().HandleAsync(
            new EntregarOrdenCommand(_orden.Id, OpcionesDeGarantia.Dias15, 10.005m, MetodosDePago.Efectivo), default);

        Assert.Equal("entrega_invalida", resultado.Error!.Codigo);
        AssertSinCambios(EstadosDeReparacion.ListoParaRetirar);
    }
}
