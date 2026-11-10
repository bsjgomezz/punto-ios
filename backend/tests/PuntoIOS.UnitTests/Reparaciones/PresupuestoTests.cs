using PuntoIOS.Application.Common.Results;
using PuntoIOS.Application.Features.Reparaciones.CrearVersionPresupuesto;
using PuntoIOS.Domain.Auditoria;
using PuntoIOS.Domain.Clientes;
using PuntoIOS.Domain.Reparaciones;
using PuntoIOS.UnitTests.Clientes;
using PuntoIOS.UnitTests.Identidad;

namespace PuntoIOS.UnitTests.Reparaciones;

public class VersionDePresupuestoTests
{
    private static readonly DateTimeOffset Ahora = new(2026, 11, 8, 12, 0, 0, TimeSpan.Zero);

    private static VersionDePresupuesto Crear(List<ItemDePresupuesto> items, int numero = 1) =>
        VersionDePresupuesto.Crear(Guid.NewGuid(), Guid.NewGuid(), numero, items, Guid.NewGuid(), Ahora);

    public static TheoryData<string, string, decimal> ItemsInvalidos => new()
    {
        { "Pantalla", "regalo", 100m },
        { "Pantalla", "", 100m },
        { "", TiposDeItemPresupuesto.Repuesto, 100m },
        { "   ", TiposDeItemPresupuesto.Repuesto, 100m },
        { new string('x', ItemDePresupuesto.LargoMaximoDescripcion + 1), TiposDeItemPresupuesto.Repuesto, 100m },
        { "Pantalla", TiposDeItemPresupuesto.Repuesto, 0m },
        { "Pantalla", TiposDeItemPresupuesto.Repuesto, -1m },
        { "Pantalla", TiposDeItemPresupuesto.Repuesto, 10.001m },
        { "Pantalla", TiposDeItemPresupuesto.Repuesto, ItemDePresupuesto.MontoMaximo + 0.01m },
    };

    [Fact]
    public void Sin_items_no_se_puede_crear()
    {
        Assert.Throws<ArgumentException>(() => Crear([]));
    }

    [Fact]
    public void Con_mas_items_que_el_maximo_no_se_puede_crear()
    {
        var items = Enumerable.Range(0, VersionDePresupuesto.MaximoItems + 1)
            .Select(i => new ItemDePresupuesto($"Ítem {i}", TiposDeItemPresupuesto.ManoDeObra, 10m))
            .ToList();

        Assert.Throws<ArgumentException>(() => Crear(items));
    }

    [Fact]
    public void Con_el_maximo_de_items_se_puede_crear()
    {
        var items = Enumerable.Range(0, VersionDePresupuesto.MaximoItems)
            .Select(i => new ItemDePresupuesto($"Ítem {i}", TiposDeItemPresupuesto.ManoDeObra, 10m))
            .ToList();

        Assert.Equal(VersionDePresupuesto.MaximoItems * 10m, Crear(items).Total);
    }

    [Theory]
    [MemberData(nameof(ItemsInvalidos))]
    public void Un_item_invalido_impide_crear_la_version(string descripcion, string tipo, decimal monto)
    {
        List<ItemDePresupuesto> items =
        [
            new("Mano de obra", TiposDeItemPresupuesto.ManoDeObra, 5000m),
            new(descripcion, tipo, monto),
        ];

        Assert.Throws<ArgumentException>(() => Crear(items));
    }

    [Fact]
    public void Una_version_valida_suma_el_total_y_guarda_las_descripciones_sin_espacios_de_mas()
    {
        var localId = Guid.NewGuid();
        var ordenId = Guid.NewGuid();
        var usuarioId = Guid.NewGuid();

        var version = VersionDePresupuesto.Crear(
            localId,
            ordenId,
            3,
            [
                new ItemDePresupuesto("  Pantalla OLED  ", TiposDeItemPresupuesto.Repuesto, 15000m),
                new ItemDePresupuesto("Cambio de pantalla", TiposDeItemPresupuesto.ManoDeObra, 2500.50m),
            ],
            usuarioId,
            Ahora);

        Assert.Equal(17500.50m, version.Total);
        Assert.Equal((localId, ordenId, 3, usuarioId, Ahora), (version.LocalId, version.OrdenId, version.Numero, version.CreadoPorUsuarioId, version.CreadoEn));
        Assert.NotEqual(Guid.Empty, version.Id);
        Assert.Equal(
            [
                new ItemDePresupuesto("Pantalla OLED", TiposDeItemPresupuesto.Repuesto, 15000m),
                new ItemDePresupuesto("Cambio de pantalla", TiposDeItemPresupuesto.ManoDeObra, 2500.50m),
            ],
            version.Items);
    }
}

public class CrearVersionPresupuestoHandlerTests
{
    private static readonly DateTimeOffset Ahora = new(2026, 11, 8, 12, 0, 0, TimeSpan.Zero);
    private static readonly Guid LocalId = Guid.NewGuid();
    private static readonly Guid UsuarioId = Guid.NewGuid();

    /// <summary>Camino válido desde "recibido" hasta cada estado que usan los tests.</summary>
    private static readonly Dictionary<string, string[]> Caminos = new()
    {
        [EstadosDeReparacion.Recibido] = [],
        [EstadosDeReparacion.EnDiagnostico] = [EstadosDeReparacion.EnPruebas, EstadosDeReparacion.EnDiagnostico],
        [EstadosDeReparacion.EsperandoPresupuesto] =
            [EstadosDeReparacion.EnPruebas, EstadosDeReparacion.EnDiagnostico, EstadosDeReparacion.EsperandoPresupuesto],
        [EstadosDeReparacion.PresupuestoEnviado] =
        [
            EstadosDeReparacion.EnPruebas, EstadosDeReparacion.EnDiagnostico, EstadosDeReparacion.EsperandoPresupuesto,
            EstadosDeReparacion.PresupuestoEnviado,
        ],
        [EstadosDeReparacion.EsperandoAprobacion] =
        [
            EstadosDeReparacion.EnPruebas, EstadosDeReparacion.EnDiagnostico, EstadosDeReparacion.EsperandoPresupuesto,
            EstadosDeReparacion.PresupuestoEnviado, EstadosDeReparacion.EsperandoAprobacion,
        ],
        [EstadosDeReparacion.PresupuestoRechazado] =
        [
            EstadosDeReparacion.EnPruebas, EstadosDeReparacion.EnDiagnostico, EstadosDeReparacion.EsperandoPresupuesto,
            EstadosDeReparacion.PresupuestoEnviado, EstadosDeReparacion.EsperandoAprobacion, EstadosDeReparacion.PresupuestoRechazado,
        ],
        [EstadosDeReparacion.Aprobado] =
        [
            EstadosDeReparacion.EnPruebas, EstadosDeReparacion.EnDiagnostico, EstadosDeReparacion.EsperandoPresupuesto,
            EstadosDeReparacion.PresupuestoEnviado, EstadosDeReparacion.EsperandoAprobacion, EstadosDeReparacion.Aprobado,
        ],
        [EstadosDeReparacion.EnReparacion] =
        [
            EstadosDeReparacion.EnPruebas, EstadosDeReparacion.EnDiagnostico, EstadosDeReparacion.EsperandoPresupuesto,
            EstadosDeReparacion.PresupuestoEnviado, EstadosDeReparacion.EsperandoAprobacion, EstadosDeReparacion.Aprobado,
            EstadosDeReparacion.EnReparacion,
        ],
        [EstadosDeReparacion.Entregado] =
        [
            EstadosDeReparacion.EnPruebas, EstadosDeReparacion.EnDiagnostico, EstadosDeReparacion.EsperandoPresupuesto,
            EstadosDeReparacion.PresupuestoEnviado, EstadosDeReparacion.EsperandoAprobacion, EstadosDeReparacion.PresupuestoRechazado,
            EstadosDeReparacion.Entregado,
        ],
        [EstadosDeReparacion.Cancelado] = [EstadosDeReparacion.Cancelado],
    };

    private readonly ClienteRepoEnMemoria _clientes = new();
    private readonly DispositivoRepoEnMemoria _dispositivos = new();
    private readonly AuditoriaRepoEnMemoria _auditoria = new();
    private readonly OrdenRepoEnMemoria _ordenes = new();
    private readonly OrdenReparacion _orden;

    public CrearVersionPresupuestoHandlerTests()
    {
        var ana = Cliente.Registrar(LocalId, "Ana", "Pérez", "3498 111111", null, null, null, null, Ahora);
        _clientes.Clientes.Add(ana);
        var iphone = Dispositivo.Registrar(LocalId, ana.Id, "Apple", "iPhone 13", null, null, null, null, null, Ahora);
        _dispositivos.Dispositivos.Add(iphone);
        _orden = OrdenReparacion.Recibir(
            LocalId, ana.Id, iphone.Id, "Pantalla rota", [TiposDeReparacion.Pantalla], null, null, null,
            null, null, null, Ahora);
        _ordenes.Ordenes.Add(_orden);
    }

    private static List<ItemPresupuestoInput> ItemsValidos() =>
    [
        new("Pantalla OLED", TiposDeItemPresupuesto.Repuesto, 15000m),
        new("Cambio de pantalla", TiposDeItemPresupuesto.ManoDeObra, 2500.50m),
    ];

    private CrearVersionPresupuestoHandler Crear() =>
        new(_ordenes, _clientes, _dispositivos, _auditoria, new DirectorioFijo(new Dictionary<Guid, string> { [UsuarioId] = "Juan" }),
            new LocalFijo(LocalId), new UsuarioFijo(UsuarioId), new RelojFijo(Ahora));

    private void Llevar(string estado)
    {
        foreach (var paso in Caminos[estado])
        {
            _orden.CambiarEstado(paso);
        }

        Assert.Equal(estado, _orden.Estado);
    }

    private static CambioDeCampo[] CambiosDeEstado(params string[] estados) =>
        estados.Zip(estados.Skip(1), (anterior, nuevo) => new CambioDeCampo("estado", anterior, nuevo)).ToArray();

    [Fact]
    public async Task Una_orden_inexistente_devuelve_no_encontrado()
    {
        var resultado = await Crear().HandleAsync(new CrearVersionPresupuestoCommand(Guid.NewGuid(), ItemsValidos()), default);

        Assert.Equal(TipoError.NoEncontrado, resultado.Error!.Tipo);
        Assert.Equal("orden_inexistente", resultado.Error.Codigo);
        Assert.Empty(_auditoria.Registros);
        Assert.Empty(_ordenes.VersionesPresupuesto);
    }

    [Theory]
    [InlineData(EstadosDeReparacion.Entregado)]
    [InlineData(EstadosDeReparacion.Cancelado)]
    public async Task Una_orden_entregada_o_cancelada_no_se_presupuesta(string estado)
    {
        Llevar(estado);

        var resultado = await Crear().HandleAsync(new CrearVersionPresupuestoCommand(_orden.Id, ItemsValidos()), default);

        Assert.Equal(TipoError.Conflicto, resultado.Error!.Tipo);
        Assert.Equal("orden_finalizada", resultado.Error.Codigo);
        Assert.Equal(estado, _orden.Estado);
        Assert.Empty(_auditoria.Registros);
        Assert.Empty(_ordenes.VersionesPresupuesto);
        Assert.Equal(0, _ordenes.Guardados);
    }

    [Fact]
    public async Task Items_invalidos_se_rechazan_sin_tocar_nada()
    {
        Llevar(EstadosDeReparacion.EsperandoPresupuesto);
        List<ItemPresupuestoInput>?[] casos =
        [
            null,
            [],
            [new("Pantalla", TiposDeItemPresupuesto.Repuesto, -100m)],
            [new(null, TiposDeItemPresupuesto.Repuesto, 100m)],
            [new("Pantalla", null, 100m)],
            Enumerable.Range(0, VersionDePresupuesto.MaximoItems + 1)
                .Select(i => new ItemPresupuestoInput($"Ítem {i}", TiposDeItemPresupuesto.ManoDeObra, 10m))
                .ToList(),
        ];

        foreach (var items in casos)
        {
            var resultado = await Crear().HandleAsync(new CrearVersionPresupuestoCommand(_orden.Id, items), default);

            Assert.Equal(TipoError.Validacion, resultado.Error!.Tipo);
            Assert.Equal("presupuesto_invalido", resultado.Error.Codigo);
            Assert.DoesNotContain("(Parameter", resultado.Error.Mensaje);
        }

        Assert.Equal(EstadosDeReparacion.EsperandoPresupuesto, _orden.Estado);
        Assert.Empty(_auditoria.Registros);
        Assert.Empty(_ordenes.VersionesPresupuesto);
        Assert.Equal(0, _ordenes.Guardados);
    }

    [Fact]
    public async Task Desde_esperando_presupuesto_guarda_la_version_y_pasa_a_presupuesto_enviado()
    {
        Llevar(EstadosDeReparacion.EsperandoPresupuesto);

        var resultado = await Crear().HandleAsync(new CrearVersionPresupuestoCommand(_orden.Id, ItemsValidos()), default);

        Assert.True(resultado.EsExitoso);
        Assert.Equal(EstadosDeReparacion.PresupuestoEnviado, resultado.Valor!.Estado);
        Assert.Equal(1, _ordenes.Guardados);

        var version = Assert.Single(_ordenes.VersionesPresupuesto);
        Assert.Equal((1, 17500.50m, LocalId, _orden.Id), (version.Numero, version.Total, version.LocalId, version.OrdenId));

        var vigente = resultado.Valor.PresupuestoVigente!;
        Assert.Equal((version.Id, 1, 17500.50m, "Juan", Ahora), (vigente.Id, vigente.Numero, vigente.Total, vigente.CreadoPor, vigente.CreadoEn));
        Assert.Equal(2, vigente.Items.Count);
        Assert.Equal("Pantalla OLED", vigente.Items[0].Descripcion);

        Assert.Equal(2, _auditoria.Registros.Count);
        var creado = _auditoria.Registros[0];
        Assert.Equal(AccionesAuditadas.PresupuestoCreado, creado.Accion);
        Assert.Equal((EntidadesAuditadas.Reparacion, _orden.Id, UsuarioId, Ahora), (creado.Entidad, creado.EntidadId, creado.UsuarioId, creado.Fecha));
        Assert.Equal(
            [new CambioDeCampo("version_presupuesto", null, "1"), new CambioDeCampo("total_presupuesto", null, "17500.50")],
            creado.Cambios);

        var estado = _auditoria.Registros[1];
        Assert.Equal(AccionesAuditadas.EstadoCambiado, estado.Accion);
        Assert.Equal((EntidadesAuditadas.Reparacion, _orden.Id, UsuarioId, Ahora), (estado.Entidad, estado.EntidadId, estado.UsuarioId, estado.Fecha));
        Assert.Equal(
            CambiosDeEstado(EstadosDeReparacion.EsperandoPresupuesto, EstadosDeReparacion.PresupuestoEnviado),
            estado.Cambios);
    }

    [Fact]
    public async Task Desde_presupuesto_rechazado_pasa_por_esperando_presupuesto_y_termina_en_presupuesto_enviado()
    {
        Llevar(EstadosDeReparacion.PresupuestoRechazado);

        var resultado = await Crear().HandleAsync(new CrearVersionPresupuestoCommand(_orden.Id, ItemsValidos()), default);

        Assert.True(resultado.EsExitoso);
        Assert.Equal(EstadosDeReparacion.PresupuestoEnviado, _orden.Estado);
        Assert.Equal(1, _ordenes.Guardados);

        Assert.Equal(
            [AccionesAuditadas.PresupuestoCreado, AccionesAuditadas.EstadoCambiado, AccionesAuditadas.EstadoCambiado],
            _auditoria.Registros.Select(r => r.Accion));
        var pasos = CambiosDeEstado(
            EstadosDeReparacion.PresupuestoRechazado, EstadosDeReparacion.EsperandoPresupuesto, EstadosDeReparacion.PresupuestoEnviado);
        Assert.Equal(pasos[0], Assert.Single(_auditoria.Registros[1].Cambios));
        Assert.Equal(pasos[1], Assert.Single(_auditoria.Registros[2].Cambios));
    }

    [Theory]
    [InlineData(EstadosDeReparacion.Aprobado)]
    [InlineData(EstadosDeReparacion.EnReparacion)]
    public async Task Desde_aprobado_o_en_reparacion_vuelve_a_esperando_aprobacion(string estado)
    {
        Llevar(estado);

        var resultado = await Crear().HandleAsync(new CrearVersionPresupuestoCommand(_orden.Id, ItemsValidos()), default);

        Assert.True(resultado.EsExitoso);
        Assert.Equal(EstadosDeReparacion.EsperandoAprobacion, resultado.Valor!.Estado);
        Assert.Equal(2, _auditoria.Registros.Count);
        Assert.Equal(AccionesAuditadas.PresupuestoCreado, _auditoria.Registros[0].Accion);
        Assert.Equal(AccionesAuditadas.EstadoCambiado, _auditoria.Registros[1].Accion);
        Assert.Equal(
            CambiosDeEstado(estado, EstadosDeReparacion.EsperandoAprobacion),
            _auditoria.Registros[1].Cambios);
    }

    [Theory]
    [InlineData(EstadosDeReparacion.Recibido)]
    [InlineData(EstadosDeReparacion.EnDiagnostico)]
    [InlineData(EstadosDeReparacion.PresupuestoEnviado)]
    [InlineData(EstadosDeReparacion.EsperandoAprobacion)]
    public async Task En_otro_estado_guarda_la_version_sin_cambiar_el_estado(string estado)
    {
        Llevar(estado);

        var resultado = await Crear().HandleAsync(new CrearVersionPresupuestoCommand(_orden.Id, ItemsValidos()), default);

        Assert.True(resultado.EsExitoso);
        Assert.Equal(estado, resultado.Valor!.Estado);
        Assert.Single(_ordenes.VersionesPresupuesto);
        Assert.Equal(1, _ordenes.Guardados);
        Assert.Equal(AccionesAuditadas.PresupuestoCreado, Assert.Single(_auditoria.Registros).Accion);
    }

    [Fact]
    public async Task Una_segunda_version_toma_el_numero_siguiente_y_registra_la_version_y_el_total_anteriores()
    {
        Llevar(EstadosDeReparacion.EnDiagnostico);
        await Crear().HandleAsync(new CrearVersionPresupuestoCommand(_orden.Id, ItemsValidos()), default);

        var resultado = await Crear().HandleAsync(
            new CrearVersionPresupuestoCommand(_orden.Id, [new("Pantalla original", TiposDeItemPresupuesto.Repuesto, 30000m)]),
            default);

        Assert.True(resultado.EsExitoso);
        Assert.Equal(2, resultado.Valor!.PresupuestoVigente!.Numero);
        Assert.Equal(30000m, resultado.Valor.PresupuestoVigente.Total);
        Assert.Equal([2, 1], (await _ordenes.ListarVersionesPresupuestoAsync(_orden.Id, default)).Select(v => v.Numero));

        var registro = _auditoria.Registros[^1];
        Assert.Equal(AccionesAuditadas.PresupuestoCreado, registro.Accion);
        Assert.Equal(
            [new CambioDeCampo("version_presupuesto", "1", "2"), new CambioDeCampo("total_presupuesto", "17500.50", "30000.00")],
            registro.Cambios);
    }
}
