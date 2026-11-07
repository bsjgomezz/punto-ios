using PuntoIOS.Application.Common.Results;
using PuntoIOS.Application.Features.Clientes;
using PuntoIOS.Application.Features.Dispositivos.ActualizarDispositivo;
using PuntoIOS.Application.Features.Dispositivos.CrearDispositivo;
using PuntoIOS.Application.Features.Dispositivos.ListarDispositivos;
using PuntoIOS.Application.Features.Dispositivos.RevelarImei;
using PuntoIOS.Domain.Auditoria;
using PuntoIOS.Domain.Clientes;
using PuntoIOS.UnitTests.Identidad;

namespace PuntoIOS.UnitTests.Clientes;

public class DispositivoHandlersTests
{
    private static readonly DateTimeOffset Ahora = new(2026, 11, 7, 12, 0, 0, TimeSpan.Zero);
    private static readonly Guid LocalId = Guid.NewGuid();
    private static readonly Guid UsuarioId = Guid.NewGuid();
    private const string Imei = "490154203237518";
    private const string OtroImei = "356938035643809";

    private readonly ClienteRepoEnMemoria _clientes = new();
    private readonly DispositivoRepoEnMemoria _dispositivos = new();
    private readonly AuditoriaRepoEnMemoria _auditoria = new();
    private readonly Cliente _ana;
    private readonly Cliente _luis;

    public DispositivoHandlersTests()
    {
        _ana = Cliente.Registrar(LocalId, "Ana", "Pérez", "3498 111111", null, null, null, null, Ahora);
        _luis = Cliente.Registrar(LocalId, "Luis", "Gómez", "3498 222222", null, null, null, null, Ahora);
        _clientes.Clientes.AddRange([_ana, _luis]);
    }

    private CrearDispositivoHandler Crear() =>
        new(_clientes, _dispositivos, _auditoria, new LocalFijo(LocalId), new UsuarioFijo(UsuarioId), new RelojFijo(Ahora));

    private ActualizarDispositivoHandler Editar() =>
        new(_dispositivos, _clientes, _auditoria, new LocalFijo(LocalId), new UsuarioFijo(UsuarioId), new RelojFijo(Ahora));

    private static CrearDispositivoCommand Alta(Guid clienteId, string? imei = Imei, string? modelo = "iPhone 13") =>
        new(clienteId, "Apple", modelo, "128 GB", "Azul", imei, null);

    [Fact]
    public async Task Registra_el_equipo_con_imei_enmascarado_y_deja_constancia()
    {
        var resultado = await Crear().HandleAsync(Alta(_ana.Id), default);

        Assert.True(resultado.EsExitoso);
        Assert.Equal("•••••••••••7518", resultado.Valor!.ImeiSerieEnmascarado);
        var registro = Assert.Single(_auditoria.Registros);
        Assert.Equal((EntidadesAuditadas.Dispositivo, AccionesAuditadas.Creado, UsuarioId), (registro.Entidad, registro.Accion, registro.UsuarioId));
        Assert.Equal(1, _dispositivos.Guardados);
    }

    [Fact]
    public async Task El_imei_es_opcional()
    {
        var resultado = await Crear().HandleAsync(Alta(_ana.Id, imei: "  "), default);

        Assert.True(resultado.EsExitoso);
        Assert.Null(resultado.Valor!.ImeiSerieEnmascarado);
    }

    [Fact]
    public async Task Un_imei_ya_registrado_no_se_permite_e_indica_de_quien_es()
    {
        await Crear().HandleAsync(Alta(_ana.Id), default);

        var resultado = await Crear().HandleAsync(Alta(_luis.Id, imei: "49-0154-2032-3751-8"), default);

        Assert.Equal(TipoError.Conflicto, resultado.Error!.Tipo);
        Assert.Equal("imei_duplicado", resultado.Error.Codigo);
        var duenio = Assert.IsType<ClienteResumenDto>(resultado.Error.Datos!["cliente"]);
        Assert.Equal("Ana", duenio.Nombre);
        Assert.Single(_dispositivos.Dispositivos);
    }

    [Theory]
    [InlineData(null, "iPhone 13", Imei, "marca_obligatoria")]
    [InlineData("Apple", " ", Imei, "modelo_obligatorio")]
    [InlineData("Apple", "iPhone 13", "490154203237519", "imei_invalido")]
    [InlineData("Apple", "iPhone 13", "123", "imei_invalido")]
    public async Task Valida_los_datos(string? marca, string? modelo, string? imei, string codigo)
    {
        var resultado = await Crear().HandleAsync(new CrearDispositivoCommand(_ana.Id, marca, modelo, null, null, imei, null), default);

        Assert.Equal(codigo, resultado.Error!.Codigo);
        Assert.Empty(_dispositivos.Dispositivos);
    }

    [Fact]
    public async Task Cliente_inexistente_responde_no_encontrado()
    {
        var alta = await Crear().HandleAsync(Alta(Guid.NewGuid()), default);
        var lista = await new ListarDispositivosHandler(_clientes, _dispositivos).HandleAsync(Guid.NewGuid(), default);

        Assert.Equal(TipoError.NoEncontrado, alta.Error!.Tipo);
        Assert.Equal(TipoError.NoEncontrado, lista.Error!.Tipo);
    }

    [Fact]
    public async Task Editar_sin_tocar_el_imei_no_lo_controla_ni_lo_cambia()
    {
        var creado = (await Crear().HandleAsync(Alta(_ana.Id), default)).Valor!;

        var resultado = await Editar().HandleAsync(
            new ActualizarDispositivoCommand(creado.Id, "Apple", "iPhone 13 Pro", "128 GB", "Azul", null, false, null), default);

        Assert.True(resultado.EsExitoso);
        Assert.Equal("iPhone 13 Pro", resultado.Valor!.Modelo);
        Assert.Equal(Imei, _dispositivos.Dispositivos[0].ImeiSerie);
        Assert.Equal(AccionesAuditadas.Editado, _auditoria.Registros[^1].Accion);
    }

    [Fact]
    public async Task Editar_con_el_imei_de_otro_equipo_no_se_permite_pero_con_el_propio_si()
    {
        var deAna = (await Crear().HandleAsync(Alta(_ana.Id), default)).Valor!;
        await Crear().HandleAsync(Alta(_luis.Id, imei: OtroImei), default);

        var ajeno = await Editar().HandleAsync(
            new ActualizarDispositivoCommand(deAna.Id, "Apple", "iPhone 13", "128 GB", "Azul", null, true, OtroImei), default);
        var propio = await Editar().HandleAsync(
            new ActualizarDispositivoCommand(deAna.Id, "Apple", "iPhone 13", "128 GB", "Azul", null, true, Imei), default);

        Assert.Equal("imei_duplicado", ajeno.Error!.Codigo);
        Assert.True(propio.EsExitoso);
        Assert.Equal(Imei, _dispositivos.Dispositivos[0].ImeiSerie);
    }

    [Fact]
    public async Task Revelar_el_imei_devuelve_el_completo_y_deja_registrado_quien_lo_vio()
    {
        var creado = (await Crear().HandleAsync(Alta(_ana.Id), default)).Valor!;
        var handler = new RevelarImeiHandler(_dispositivos, _auditoria, new LocalFijo(LocalId), new UsuarioFijo(UsuarioId), new RelojFijo(Ahora));

        var resultado = await handler.HandleAsync(creado.Id, default);

        Assert.Equal(Imei, resultado.Valor!.ImeiSerie);
        var registro = _auditoria.Registros[^1];
        Assert.Equal((AccionesAuditadas.ImeiConsultado, UsuarioId), (registro.Accion, registro.UsuarioId));
    }
}
