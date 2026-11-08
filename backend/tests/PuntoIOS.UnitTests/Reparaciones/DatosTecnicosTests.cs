using PuntoIOS.Application.Common.Results;
using PuntoIOS.Application.Features.Reparaciones.ActualizarDatosTecnicos;
using PuntoIOS.Domain.Auditoria;
using PuntoIOS.Domain.Clientes;
using PuntoIOS.Domain.Reparaciones;
using PuntoIOS.UnitTests.Clientes;
using PuntoIOS.UnitTests.Identidad;

namespace PuntoIOS.UnitTests.Reparaciones;

public class DatosTecnicosTests
{
    private static readonly DateTimeOffset Ahora = new(2026, 11, 8, 12, 0, 0, TimeSpan.Zero);
    private static readonly Guid LocalId = Guid.NewGuid();

    private static OrdenReparacion NuevaOrden() =>
        OrdenReparacion.Recibir(
            LocalId, Guid.NewGuid(), Guid.NewGuid(), "No carga", [TiposDeReparacion.ConectorDeCarga], null, null, null,
            null, null, null, Ahora);

    [Fact]
    public void Una_orden_nueva_no_tiene_datos_tecnicos()
    {
        var orden = NuevaOrden();

        Assert.Null(orden.Pruebas);
        Assert.Null(orden.Diagnostico);
        Assert.Null(orden.Mediciones);
        Assert.Empty(orden.ComponentesAfectados);
        Assert.Null(orden.NotasInternas);
    }

    [Fact]
    public void Si_cambia_un_solo_campo_solo_ese_aparece_en_los_cambios()
    {
        var orden = NuevaOrden();
        orden.ActualizarDatosTecnicos("Carga con otro cable", null, "5,1 V", ["Pantalla"], "Cliente apurado");

        var cambios = orden.ActualizarDatosTecnicos("Carga con otro cable", "Conector sulfatado", "5,1 V", ["Pantalla"], "Cliente apurado");

        Assert.Equal([new CambioDeCampo("diagnostico", null, "Conector sulfatado")], cambios);
        Assert.Equal("Conector sulfatado", orden.Diagnostico);
    }

    [Fact]
    public void Carga_todos_los_campos_y_registra_cada_uno()
    {
        var orden = NuevaOrden();

        var cambios = orden.ActualizarDatosTecnicos(
            "  Prueba de carga  ", "Conector roto", "0,2 A", ["Conector de carga", "IC de carga"], "No cobrar el envío");

        Assert.Equal(
            [
                new CambioDeCampo("pruebas", null, "Prueba de carga"),
                new CambioDeCampo("diagnostico", null, "Conector roto"),
                new CambioDeCampo("mediciones", null, "0,2 A"),
                new CambioDeCampo("componentes_afectados", null, "Conector de carga, IC de carga"),
                new CambioDeCampo("notas_internas", null, "No cobrar el envío"),
            ],
            cambios);
        Assert.Equal("Prueba de carga", orden.Pruebas);
        Assert.Equal(["Conector de carga", "IC de carga"], orden.ComponentesAfectados);
    }

    [Fact]
    public void Si_no_cambia_nada_no_hay_cambios()
    {
        var orden = NuevaOrden();
        orden.ActualizarDatosTecnicos("Prueba", "Diag", "Med", ["Pantalla", "Batería"], "Nota");

        var cambios = orden.ActualizarDatosTecnicos(" Prueba ", "Diag", "Med", [" Pantalla", "Batería "], "Nota");

        Assert.Empty(cambios);
    }

    [Fact]
    public void Vaciar_campos_los_deja_en_null_y_la_lista_vacia_se_registra_como_null()
    {
        var orden = NuevaOrden();
        orden.ActualizarDatosTecnicos("Prueba", null, null, ["Pantalla"], null);

        var cambios = orden.ActualizarDatosTecnicos("   ", null, null, null, "");

        Assert.Equal(
            [
                new CambioDeCampo("pruebas", "Prueba", null),
                new CambioDeCampo("componentes_afectados", "Pantalla", null),
            ],
            cambios);
        Assert.Null(orden.Pruebas);
        Assert.Empty(orden.ComponentesAfectados);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void Un_texto_que_excede_el_maximo_se_rechaza_sin_cambiar_nada(int campo)
    {
        var orden = NuevaOrden();
        var largo = new string('x', OrdenReparacion.LargoMaximoTextoTecnico + 1);
        var textos = new string?[] { "ok", "ok", "ok", "ok" };
        textos[campo] = largo;

        Assert.Throws<ArgumentException>(() =>
            orden.ActualizarDatosTecnicos(textos[0], textos[1], textos[2], ["Pantalla"], textos[3]));

        Assert.Null(orden.Pruebas);
        Assert.Null(orden.Diagnostico);
        Assert.Null(orden.Mediciones);
        Assert.Null(orden.NotasInternas);
        Assert.Empty(orden.ComponentesAfectados);
    }

    [Fact]
    public void Un_texto_justo_en_el_maximo_se_acepta()
    {
        var orden = NuevaOrden();
        var justo = new string('x', OrdenReparacion.LargoMaximoTextoTecnico);

        var cambios = orden.ActualizarDatosTecnicos(justo, null, null, null, null);

        Assert.Single(cambios);
    }

    [Fact]
    public void Mas_componentes_que_el_maximo_se_rechaza()
    {
        var orden = NuevaOrden();
        var componentes = Enumerable.Range(1, OrdenReparacion.MaximoComponentesAfectados + 1).Select(i => $"Componente {i}").ToList();

        Assert.Throws<ArgumentException>(() => orden.ActualizarDatosTecnicos(null, null, null, componentes, null));
        Assert.Empty(orden.ComponentesAfectados);
    }

    [Fact]
    public void Justo_el_maximo_de_componentes_se_acepta_y_los_repetidos_no_cuentan()
    {
        var orden = NuevaOrden();
        var componentes = Enumerable.Range(1, OrdenReparacion.MaximoComponentesAfectados).Select(i => $"Componente {i}").ToList();
        componentes.Add("Componente 1");

        orden.ActualizarDatosTecnicos(null, null, null, componentes, null);

        Assert.Equal(OrdenReparacion.MaximoComponentesAfectados, orden.ComponentesAfectados.Count);
    }

    [Fact]
    public void Un_componente_demasiado_largo_se_rechaza()
    {
        var orden = NuevaOrden();
        var largo = new string('x', OrdenReparacion.LargoMaximoComponenteAfectado + 1);

        Assert.Throws<ArgumentException>(() => orden.ActualizarDatosTecnicos(null, null, null, ["Pantalla", largo], null));
        Assert.Empty(orden.ComponentesAfectados);
    }

    [Fact]
    public void Los_componentes_vacios_o_con_espacios_se_descartan()
    {
        var orden = NuevaOrden();

        orden.ActualizarDatosTecnicos(null, null, null, ["", "   ", "Pantalla", "\t"], null);

        Assert.Equal(["Pantalla"], orden.ComponentesAfectados);
    }

    [Fact]
    public void Los_componentes_repetidos_se_deduplican_conservando_la_primera_aparicion()
    {
        var orden = NuevaOrden();

        orden.ActualizarDatosTecnicos(
            null, null, null, ["Pantalla", "Batería", "  Pantalla ", "IC de carga", "Batería"], null);

        Assert.Equal(["Pantalla", "Batería", "IC de carga"], orden.ComponentesAfectados);
    }

    [Fact]
    public void Cambiar_el_orden_de_los_componentes_cuenta_como_cambio()
    {
        var orden = NuevaOrden();
        orden.ActualizarDatosTecnicos(null, null, null, ["Pantalla", "Batería"], null);

        var cambios = orden.ActualizarDatosTecnicos(null, null, null, ["Batería", "Pantalla"], null);

        Assert.Equal([new CambioDeCampo("componentes_afectados", "Pantalla, Batería", "Batería, Pantalla")], cambios);
    }
}

public class ActualizarDatosTecnicosHandlerTests
{
    private static readonly DateTimeOffset Ahora = new(2026, 11, 8, 12, 0, 0, TimeSpan.Zero);
    private static readonly Guid LocalId = Guid.NewGuid();
    private static readonly Guid UsuarioId = Guid.NewGuid();

    private readonly ClienteRepoEnMemoria _clientes = new();
    private readonly DispositivoRepoEnMemoria _dispositivos = new();
    private readonly AuditoriaRepoEnMemoria _auditoria = new();
    private readonly OrdenRepoEnMemoria _ordenes = new();
    private readonly OrdenReparacion _orden;

    public ActualizarDatosTecnicosHandlerTests()
    {
        var ana = Cliente.Registrar(LocalId, "Ana", "Pérez", "3498 111111", null, null, null, null, Ahora);
        _clientes.Clientes.Add(ana);
        var iphone = Dispositivo.Registrar(LocalId, ana.Id, "Apple", "iPhone 13", null, null, null, null, null, Ahora);
        _dispositivos.Dispositivos.Add(iphone);
        _orden = OrdenReparacion.Recibir(
            LocalId, ana.Id, iphone.Id, "No carga", [TiposDeReparacion.ConectorDeCarga], null, null, null,
            null, null, null, Ahora);
        _ordenes.Ordenes.Add(_orden);
    }

    private ActualizarDatosTecnicosHandler Crear() =>
        new(_ordenes, _clientes, _dispositivos, _auditoria, new DirectorioFijo([]),
            new LocalFijo(LocalId), new UsuarioFijo(UsuarioId), new RelojFijo(Ahora));

    [Fact]
    public async Task Una_orden_inexistente_devuelve_no_encontrado()
    {
        var resultado = await Crear().HandleAsync(
            new ActualizarDatosTecnicosCommand(Guid.NewGuid(), "Prueba", null, null, null, null), default);

        Assert.Equal("orden_inexistente", resultado.Error!.Codigo);
        Assert.Empty(_auditoria.Registros);
    }

    [Fact]
    public async Task Datos_invalidos_devuelven_error_de_validacion_sin_guardar()
    {
        var largo = new string('x', OrdenReparacion.LargoMaximoTextoTecnico + 1);

        var resultado = await Crear().HandleAsync(
            new ActualizarDatosTecnicosCommand(_orden.Id, "Prueba", largo, null, null, null), default);

        Assert.Equal(TipoError.Validacion, resultado.Error!.Tipo);
        Assert.Equal("datos_tecnicos_invalidos", resultado.Error.Codigo);
        Assert.DoesNotContain("Parameter", resultado.Error.Mensaje);
        Assert.Null(_orden.Pruebas);
        Assert.Empty(_auditoria.Registros);
        Assert.Equal(0, _ordenes.Guardados);
    }

    [Fact]
    public async Task Demasiados_componentes_devuelven_error_de_validacion()
    {
        var componentes = Enumerable.Range(1, OrdenReparacion.MaximoComponentesAfectados + 1).Select(i => $"C{i}").ToList();

        var resultado = await Crear().HandleAsync(
            new ActualizarDatosTecnicosCommand(_orden.Id, null, null, null, componentes, null), default);

        Assert.Equal("datos_tecnicos_invalidos", resultado.Error!.Codigo);
        Assert.Empty(_auditoria.Registros);
    }

    [Fact]
    public async Task Una_actualizacion_valida_devuelve_los_datos_y_deja_registro_de_los_cambios()
    {
        var resultado = await Crear().HandleAsync(
            new ActualizarDatosTecnicosCommand(_orden.Id, "Prueba de carga", "Conector roto", null, ["Conector de carga"], "Ojo con el marco"),
            default);

        Assert.True(resultado.EsExitoso);
        var dto = resultado.Valor!;
        Assert.Equal("Prueba de carga", dto.Pruebas);
        Assert.Equal("Conector roto", dto.Diagnostico);
        Assert.Null(dto.Mediciones);
        Assert.Equal(["Conector de carga"], dto.ComponentesAfectados);
        Assert.Equal("Ojo con el marco", dto.NotasInternas);
        Assert.Equal(1, _ordenes.Guardados);

        var registro = Assert.Single(_auditoria.Registros);
        Assert.Equal(AccionesAuditadas.Editado, registro.Accion);
        Assert.Equal(EntidadesAuditadas.Reparacion, registro.Entidad);
        Assert.Equal((_orden.Id, UsuarioId, Ahora), (registro.EntidadId, registro.UsuarioId, registro.Fecha));
        Assert.Equal(
            [
                new CambioDeCampo("pruebas", null, "Prueba de carga"),
                new CambioDeCampo("diagnostico", null, "Conector roto"),
                new CambioDeCampo("componentes_afectados", null, "Conector de carga"),
                new CambioDeCampo("notas_internas", null, "Ojo con el marco"),
            ],
            registro.Cambios);
    }

    [Fact]
    public async Task Si_no_cambia_nada_no_se_audita_ni_se_guarda()
    {
        var comando = new ActualizarDatosTecnicosCommand(_orden.Id, "Prueba", null, null, ["Pantalla"], null);
        await Crear().HandleAsync(comando, default);

        var resultado = await Crear().HandleAsync(comando, default);

        Assert.True(resultado.EsExitoso);
        Assert.Single(_auditoria.Registros);
        Assert.Equal(1, _ordenes.Guardados);
    }
}
