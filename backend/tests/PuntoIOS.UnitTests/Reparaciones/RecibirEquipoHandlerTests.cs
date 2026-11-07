using PuntoIOS.Application.Features.Clientes.CrearCliente;
using PuntoIOS.Application.Features.Identidad.Puertos;
using PuntoIOS.Application.Features.Reparaciones.RecibirEquipo;
using PuntoIOS.Application.Features.Reparaciones.VerDesbloqueo;
using PuntoIOS.Domain.Auditoria;
using PuntoIOS.Domain.Clientes;
using PuntoIOS.Domain.Reparaciones;
using PuntoIOS.UnitTests.Clientes;
using PuntoIOS.UnitTests.Identidad;

namespace PuntoIOS.UnitTests.Reparaciones;

public class RecibirEquipoHandlerTests
{
    private static readonly DateTimeOffset Ahora = new(2026, 11, 7, 12, 0, 0, TimeSpan.Zero);
    private static readonly Guid LocalId = Guid.NewGuid();
    private static readonly Guid UsuarioId = Guid.NewGuid();
    private static readonly UsuarioResumen Tecnico = new(Guid.NewGuid(), "Técnico 1");

    private readonly ClienteRepoEnMemoria _clientes = new();
    private readonly DispositivoRepoEnMemoria _dispositivos = new();
    private readonly AuditoriaRepoEnMemoria _auditoria = new();
    private readonly OrdenRepoEnMemoria _ordenes = new();
    private readonly Cliente _ana;
    private readonly Cliente _luis;
    private readonly Dispositivo _iphoneDeAna;

    public RecibirEquipoHandlerTests()
    {
        _ana = Cliente.Registrar(LocalId, "Ana", "Pérez", "3498 111111", null, null, null, null, Ahora);
        _luis = Cliente.Registrar(LocalId, "Luis", "Gómez", "3498 222222", null, null, null, null, Ahora);
        _clientes.Clientes.AddRange([_ana, _luis]);
        _iphoneDeAna = Dispositivo.Registrar(LocalId, _ana.Id, "Apple", "iPhone 13", null, null, "490154203237518", null, null, Ahora);
        _dispositivos.Dispositivos.Add(_iphoneDeAna);
    }

    private RecibirEquipoHandler Crear() =>
        new(
            _ordenes,
            _clientes,
            _dispositivos,
            _auditoria,
            new DirectorioFijo(new() { [Tecnico.Id] = Tecnico.Nombre, [UsuarioId] = "Recepción 1" }, [Tecnico]),
            new CifradoDePrueba(),
            new LocalFijo(LocalId),
            new UsuarioFijo(UsuarioId),
            new RelojFijo(Ahora));

    private RecibirEquipoCommand Comando(
        Guid? dispositivoId = null,
        EquipoNuevo? nuevo = null,
        string? problema = "No carga",
        IReadOnlyList<string>? tipos = null,
        string? otro = null,
        Guid? tecnico = null,
        DateOnly? fecha = null,
        string? desbloqueoTipo = null,
        string? desbloqueoValor = null) =>
        new(
            _ana.Id,
            nuevo is null ? dispositivoId ?? _iphoneDeAna.Id : dispositivoId,
            nuevo,
            problema,
            tipos ?? [TiposDeReparacion.ConectorDeCarga],
            otro,
            tecnico,
            fecha,
            desbloqueoTipo,
            desbloqueoValor);

    [Fact]
    public async Task Recibe_un_equipo_ya_registrado_y_deja_la_orden_en_recibido()
    {
        var resultado = await Crear().HandleAsync(
            Comando(tecnico: Tecnico.Id, fecha: new DateOnly(2026, 11, 10)), default);

        Assert.True(resultado.EsExitoso);
        var orden = Assert.Single(_ordenes.Ordenes);
        Assert.Equal(EstadosDeReparacion.Recibido, orden.Estado);
        Assert.Equal(_iphoneDeAna.Id, orden.DispositivoId);
        Assert.Equal("Técnico 1", resultado.Valor!.Tecnico!.Nombre);
        Assert.Equal("Recepción 1", resultado.Valor.CreadoPor);
        Assert.Equal("•••••••••••7518", resultado.Valor.Equipo.ImeiSerieEnmascarado);
        Assert.Equal(AccionesAuditadas.Creado, Assert.Single(_auditoria.Registros).Accion);
    }

    [Fact]
    public async Task Con_un_equipo_nuevo_crea_equipo_y_orden_juntos()
    {
        var resultado = await Crear().HandleAsync(
            Comando(nuevo: new EquipoNuevo("Apple", "iPhone 15", "256 GB", null, null)), default);

        Assert.True(resultado.EsExitoso);
        Assert.Equal(2, _dispositivos.Dispositivos.Count);
        Assert.Equal("iPhone 15", resultado.Valor!.Equipo.Modelo);
        Assert.Equal(
            new[] { EntidadesAuditadas.Dispositivo, EntidadesAuditadas.Reparacion },
            _auditoria.Registros.Select(r => r.Entidad));
        Assert.Equal(1, _ordenes.Guardados);
    }

    [Fact]
    public async Task El_desbloqueo_se_guarda_cifrado_y_al_verlo_queda_registrado()
    {
        await Crear().HandleAsync(
            Comando(desbloqueoTipo: TiposDeDesbloqueo.Patron, desbloqueoValor: "1 - 5 - 9 - 6"), default);
        var orden = Assert.Single(_ordenes.Ordenes);
        Assert.Equal(TiposDeDesbloqueo.Patron, orden.DesbloqueoTipo);
        Assert.DoesNotContain("1-5-9-6", orden.DesbloqueoCifrado);

        var ver = new VerDesbloqueoHandler(
            _ordenes, _auditoria, new CifradoDePrueba(), new LocalFijo(LocalId), new UsuarioFijo(Tecnico.Id), new RelojFijo(Ahora));
        var resultado = await ver.HandleAsync(orden.Id, default);

        Assert.Equal(new DesbloqueoDto(TiposDeDesbloqueo.Patron, "1-5-9-6"), resultado.Valor);
        var registro = _auditoria.Registros[^1];
        Assert.Equal((AccionesAuditadas.DesbloqueoConsultado, Tecnico.Id), (registro.Accion, registro.UsuarioId));
    }

    [Fact]
    public void Al_borrar_el_desbloqueo_no_queda_rastro()
    {
        var orden = OrdenReparacion.Recibir(
            LocalId, _ana.Id, _iphoneDeAna.Id, "x", [TiposDeReparacion.Pantalla], null, null, null,
            TiposDeDesbloqueo.Pin, "cifrado", null, Ahora);

        orden.BorrarDesbloqueo();

        Assert.False(orden.TieneDesbloqueo);
        Assert.Null(orden.DesbloqueoTipo);
    }

    public static TheoryData<string, string?, string[]?, string?, int, string?, string?> Invalidos => new()
    {
        { "problema_obligatorio", " ", null, null, 0, null, null },
        { "tipo_obligatorio", "No carga", [], null, 0, null, null },
        { "tipo_invalido", "No carga", ["teletransporte"], null, 0, null, null },
        { "otro_tipo_obligatorio", "No carga", [TiposDeReparacion.Otro], " ", 0, null, null },
        { "fecha_invalida", "No carga", null, null, -5, null, null },
        { "desbloqueo_invalido", "No carga", null, null, 0, TiposDeDesbloqueo.Pin, "12ab" },
        { "desbloqueo_invalido", "No carga", null, null, 0, TiposDeDesbloqueo.Patron, "1-2-3" },
        { "desbloqueo_invalido", "No carga", null, null, 0, TiposDeDesbloqueo.Patron, "1-2-3-1" },
        { "desbloqueo_invalido", "No carga", null, null, 0, null, "1234" },
    };

    [Theory]
    [MemberData(nameof(Invalidos))]
    public async Task Valida_los_datos_sin_guardar_nada(
        string codigo, string? problema, string[]? tipos, string? otro, int diasFecha, string? desbloqueoTipo, string? desbloqueoValor)
    {
        var fecha = diasFecha == 0 ? (DateOnly?)null : DateOnly.FromDateTime(Ahora.UtcDateTime).AddDays(diasFecha);

        var resultado = await Crear().HandleAsync(
            Comando(problema: problema, tipos: tipos, otro: otro, fecha: fecha, desbloqueoTipo: desbloqueoTipo, desbloqueoValor: desbloqueoValor),
            default);

        Assert.Equal(codigo, resultado.Error!.Codigo);
        Assert.Empty(_ordenes.Ordenes);
    }

    [Fact]
    public async Task Rechaza_un_equipo_de_otro_cliente_un_tecnico_que_no_repara_y_no_elegir_equipo()
    {
        var iphoneDeLuis = Dispositivo.Registrar(LocalId, _luis.Id, "Apple", "iPhone 12", null, null, null, null, null, Ahora);
        _dispositivos.Dispositivos.Add(iphoneDeLuis);

        var ajeno = await Crear().HandleAsync(Comando(dispositivoId: iphoneDeLuis.Id), default);
        var tecnico = await Crear().HandleAsync(Comando(tecnico: Guid.NewGuid()), default);
        var ambos = await Crear().HandleAsync(
            Comando(dispositivoId: _iphoneDeAna.Id, nuevo: new EquipoNuevo("Apple", "iPhone 15", null, null, null)), default);

        Assert.Equal("equipo_invalido", ajeno.Error!.Codigo);
        Assert.Equal("tecnico_invalido", tecnico.Error!.Codigo);
        Assert.Equal("equipo_obligatorio", ambos.Error!.Codigo);
        Assert.Empty(_ordenes.Ordenes);
    }

    [Fact]
    public async Task Un_equipo_nuevo_con_imei_ya_registrado_no_se_permite()
    {
        var resultado = await Crear().HandleAsync(
            Comando(nuevo: new EquipoNuevo("Apple", "iPhone 13", null, null, "490154203237518")), default);

        Assert.Equal("imei_duplicado", resultado.Error!.Codigo);
        Assert.Single(_dispositivos.Dispositivos);
    }

    [Theory]
    [InlineData(TiposDeDesbloqueo.Pin, " 123456 ", "123456")]
    [InlineData(TiposDeDesbloqueo.Patron, "1-5-9-6", "1-5-9-6")]
    [InlineData(TiposDeDesbloqueo.Contrasena, "  hola mundo ", "hola mundo")]
    [InlineData(TiposDeDesbloqueo.Pin, "123", null)]
    [InlineData("huella", "1234", null)]
    public void Normaliza_el_desbloqueo_segun_el_tipo(string tipo, string valor, string? esperado) =>
        Assert.Equal(esperado, TiposDeDesbloqueo.Normalizar(tipo, valor));
}
