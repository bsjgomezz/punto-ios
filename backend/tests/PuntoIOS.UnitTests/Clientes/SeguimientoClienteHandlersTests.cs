using PuntoIOS.Application.Common.Results;
using PuntoIOS.Application.Common.Security;
using PuntoIOS.Application.Features.Clientes.SeguimientoCliente;
using PuntoIOS.Domain.Auditoria;
using PuntoIOS.Domain.Clientes;
using PuntoIOS.UnitTests.Identidad;

namespace PuntoIOS.UnitTests.Clientes;

internal sealed class CifradoDePrueba : ICifradoDeDatos
{
    public string Cifrar(string texto) => "cifrado:" + new string(texto.Reverse().ToArray());

    public string Descifrar(string cifrado) => new(cifrado["cifrado:".Length..].Reverse().ToArray());
}

public class SeguimientoClienteHandlersTests
{
    private static readonly DateTimeOffset Ahora = new(2026, 11, 7, 12, 0, 0, TimeSpan.Zero);
    private static readonly Guid LocalId = Guid.NewGuid();
    private static readonly Guid UsuarioId = Guid.NewGuid();

    private readonly ClienteRepoEnMemoria _clientes = new();
    private readonly AuditoriaRepoEnMemoria _auditoria = new();
    private readonly RelojFijo _reloj = new(Ahora);
    private readonly Cliente _ana;

    public SeguimientoClienteHandlersTests()
    {
        _ana = Cliente.Registrar(LocalId, "Ana", "Pérez", "3498 111111", null, null, null, null, Ahora);
        _clientes.Clientes.Add(_ana);
    }

    private GenerarCodigoSeguimientoHandler Generar() =>
        new(_clientes, _auditoria, new CifradoDePrueba(), new LocalFijo(LocalId), new UsuarioFijo(UsuarioId), _reloj);

    private ConsultarCodigoSeguimientoHandler Consultar() =>
        new(_clientes, _auditoria, new CifradoDePrueba(), new LocalFijo(LocalId), new UsuarioFijo(UsuarioId), _reloj);

    [Fact]
    public async Task Genera_un_codigo_valido_y_guarda_solo_su_huella_y_su_version_cifrada()
    {
        var resultado = await Generar().HandleAsync(_ana.Id, default);

        var codigo = resultado.Valor!.Codigo;
        Assert.True(CodigoSeguimiento.EsValido(codigo));
        Assert.Equal(CodigoSeguimiento.Formatear(codigo), resultado.Valor.Formateado);
        Assert.Equal(CodigoSeguimiento.Huella(codigo), _ana.CodigoSeguimientoHuella);
        Assert.NotEqual(codigo, _ana.CodigoSeguimientoCifrado);
        Assert.Equal(Ahora, _ana.CodigoSeguimientoGeneradoEn);
        Assert.Equal(AccionesAuditadas.CodigoGenerado, Assert.Single(_auditoria.Registros).Accion);
    }

    [Fact]
    public async Task Regenerar_reemplaza_el_codigo_y_queda_registrado_como_regeneracion()
    {
        var primero = (await Generar().HandleAsync(_ana.Id, default)).Valor!.Codigo;
        _reloj.Ahora = Ahora.AddDays(1);

        var segundo = (await Generar().HandleAsync(_ana.Id, default)).Valor!.Codigo;

        Assert.NotEqual(primero, segundo);
        Assert.Equal(CodigoSeguimiento.Huella(segundo), _ana.CodigoSeguimientoHuella);
        Assert.Equal(AccionesAuditadas.CodigoRegenerado, _auditoria.Registros[^1].Accion);
    }

    [Fact]
    public async Task Consultar_devuelve_el_codigo_vigente_y_deja_registrado_quien_lo_vio()
    {
        var generado = (await Generar().HandleAsync(_ana.Id, default)).Valor!;

        var consultado = await Consultar().HandleAsync(_ana.Id, default);

        Assert.Equal(generado.Codigo, consultado.Valor!.Codigo);
        var registro = _auditoria.Registros[^1];
        Assert.Equal((AccionesAuditadas.CodigoConsultado, UsuarioId), (registro.Accion, registro.UsuarioId));
    }

    [Fact]
    public async Task Consultar_sin_codigo_generado_o_de_cliente_inexistente_responde_no_encontrado()
    {
        var sinCodigo = await Consultar().HandleAsync(_ana.Id, default);
        var inexistente = await Consultar().HandleAsync(Guid.NewGuid(), default);

        Assert.Equal("codigo_inexistente", sinCodigo.Error!.Codigo);
        Assert.Equal(TipoError.NoEncontrado, inexistente.Error!.Tipo);
        Assert.Empty(_auditoria.Registros);
    }
}
