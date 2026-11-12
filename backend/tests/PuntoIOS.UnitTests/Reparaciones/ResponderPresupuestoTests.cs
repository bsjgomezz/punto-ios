using PuntoIOS.Application.Common.Results;
using PuntoIOS.Application.Features.Reparaciones.ResponderPresupuesto;
using PuntoIOS.Domain.Auditoria;
using PuntoIOS.Domain.Clientes;
using PuntoIOS.Domain.Reparaciones;
using PuntoIOS.UnitTests.Clientes;
using PuntoIOS.UnitTests.Identidad;

namespace PuntoIOS.UnitTests.Reparaciones;

public class RespuestaDePresupuestoTests
{
    private static readonly DateTimeOffset Ahora = new(2026, 11, 10, 12, 0, 0, TimeSpan.Zero);

    private static VersionDePresupuesto Version() =>
        VersionDePresupuesto.Crear(
            Guid.NewGuid(), Guid.NewGuid(), 1, [new ItemDePresupuesto("Pantalla", TiposDeItemPresupuesto.Repuesto, 100m)], Guid.NewGuid(), Ahora);

    [Fact]
    public void Una_version_nueva_no_tiene_respuesta()
    {
        var version = Version();

        Assert.Null(version.RespuestaAprobada);
        Assert.Null(version.RespondidoEn);
        Assert.Null(version.RespondidoPorUsuarioId);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Una_version_sin_respuesta_registra_la_respuesta_con_hora_y_usuario(bool aprobado)
    {
        var version = Version();
        var usuarioId = Guid.NewGuid();
        var despues = Ahora.AddHours(2);

        version.RegistrarRespuesta(aprobado, usuarioId, despues);

        Assert.Equal(aprobado, version.RespuestaAprobada);
        Assert.Equal(despues, version.RespondidoEn);
        Assert.Equal(usuarioId, version.RespondidoPorUsuarioId);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Una_version_ya_respondida_no_se_puede_volver_a_responder(bool segunda)
    {
        var version = Version();
        var usuarioId = Guid.NewGuid();
        version.RegistrarRespuesta(true, usuarioId, Ahora);

        Assert.Throws<InvalidOperationException>(() => version.RegistrarRespuesta(segunda, Guid.NewGuid(), Ahora.AddHours(1)));

        // La primera respuesta queda intacta.
        Assert.True(version.RespuestaAprobada);
        Assert.Equal(Ahora, version.RespondidoEn);
        Assert.Equal(usuarioId, version.RespondidoPorUsuarioId);
    }
}

public class ResponderPresupuestoHandlerTests
{
    private static readonly DateTimeOffset Ahora = new(2026, 11, 10, 12, 0, 0, TimeSpan.Zero);
    private static readonly Guid LocalId = Guid.NewGuid();
    private static readonly Guid UsuarioId = Guid.NewGuid();

    private static readonly string[] HastaEsperandoAprobacion =
    [
        EstadosDeReparacion.EnPruebas, EstadosDeReparacion.EnDiagnostico, EstadosDeReparacion.EsperandoPresupuesto,
        EstadosDeReparacion.PresupuestoEnviado, EstadosDeReparacion.EsperandoAprobacion,
    ];

    private readonly ClienteRepoEnMemoria _clientes = new();
    private readonly DispositivoRepoEnMemoria _dispositivos = new();
    private readonly AuditoriaRepoEnMemoria _auditoria = new();
    private readonly OrdenRepoEnMemoria _ordenes = new();
    private readonly OrdenReparacion _orden;

    public ResponderPresupuestoHandlerTests()
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

    private ResponderPresupuestoHandler Crear() =>
        new(_ordenes, _clientes, _dispositivos, _auditoria, new DirectorioFijo(new Dictionary<Guid, string> { [UsuarioId] = "Juan" }),
            new LocalFijo(LocalId), new UsuarioFijo(UsuarioId), new RelojFijo(Ahora));

    private void LlevarAEsperandoAprobacion()
    {
        foreach (var paso in HastaEsperandoAprobacion)
        {
            _orden.CambiarEstado(paso);
        }
    }

    private VersionDePresupuesto AgregarVersion(int numero)
    {
        var version = VersionDePresupuesto.Crear(
            LocalId, _orden.Id, numero, [new ItemDePresupuesto("Pantalla OLED", TiposDeItemPresupuesto.Repuesto, 15000m)], UsuarioId,
            Ahora.AddDays(-1));
        _ordenes.VersionesPresupuesto.Add(version);
        return version;
    }

    private static void AssertRespuesta(VersionDePresupuesto version, bool aprobado, DateTimeOffset en, Guid por)
    {
        Assert.Equal(aprobado, version.RespuestaAprobada);
        Assert.Equal(en, version.RespondidoEn);
        Assert.Equal(por, version.RespondidoPorUsuarioId);
    }

    private void AssertSinCambios(string estado)
    {
        Assert.Equal(estado, _orden.Estado);
        Assert.Empty(_auditoria.Registros);
        Assert.Equal(0, _ordenes.Guardados);
    }

    [Fact]
    public async Task Una_orden_inexistente_devuelve_no_encontrado()
    {
        var resultado = await Crear().HandleAsync(new ResponderPresupuestoCommand(Guid.NewGuid(), true), default);

        Assert.Equal(TipoError.NoEncontrado, resultado.Error!.Tipo);
        Assert.Equal("orden_inexistente", resultado.Error.Codigo);
        Assert.Empty(_auditoria.Registros);
        Assert.Equal(0, _ordenes.Guardados);
    }

    [Fact]
    public async Task Una_orden_que_no_esta_esperando_aprobacion_devuelve_conflicto_sin_tocar_la_version()
    {
        _orden.CambiarEstado(EstadosDeReparacion.EnPruebas);
        _orden.CambiarEstado(EstadosDeReparacion.EnDiagnostico);
        _orden.CambiarEstado(EstadosDeReparacion.EsperandoPresupuesto);
        _orden.CambiarEstado(EstadosDeReparacion.PresupuestoEnviado);
        var version = AgregarVersion(1);

        var resultado = await Crear().HandleAsync(new ResponderPresupuestoCommand(_orden.Id, true), default);

        Assert.Equal(TipoError.Conflicto, resultado.Error!.Tipo);
        Assert.Equal("fuera_de_esperando_aprobacion", resultado.Error.Codigo);
        Assert.Null(version.RespuestaAprobada);
        AssertSinCambios(EstadosDeReparacion.PresupuestoEnviado);
    }

    [Fact]
    public async Task Una_orden_recien_recibida_devuelve_conflicto()
    {
        var resultado = await Crear().HandleAsync(new ResponderPresupuestoCommand(_orden.Id, false), default);

        Assert.Equal(TipoError.Conflicto, resultado.Error!.Tipo);
        Assert.Equal("fuera_de_esperando_aprobacion", resultado.Error.Codigo);
        AssertSinCambios(EstadosDeReparacion.Recibido);
    }

    [Fact]
    public async Task Sin_presupuesto_vigente_devuelve_conflicto()
    {
        // Se llegó a "esperando aprobación" con cambios de estado sueltos, sin crear ninguna versión.
        LlevarAEsperandoAprobacion();

        var resultado = await Crear().HandleAsync(new ResponderPresupuestoCommand(_orden.Id, true), default);

        Assert.Equal(TipoError.Conflicto, resultado.Error!.Tipo);
        Assert.Equal("sin_presupuesto_vigente", resultado.Error.Codigo);
        AssertSinCambios(EstadosDeReparacion.EsperandoAprobacion);
    }

    [Fact]
    public async Task Una_version_ya_respondida_devuelve_conflicto_y_conserva_la_primera_respuesta()
    {
        LlevarAEsperandoAprobacion();
        var version = AgregarVersion(1);
        var otroUsuario = Guid.NewGuid();
        version.RegistrarRespuesta(true, otroUsuario, Ahora.AddHours(-1));

        var resultado = await Crear().HandleAsync(new ResponderPresupuestoCommand(_orden.Id, false), default);

        Assert.Equal(TipoError.Conflicto, resultado.Error!.Tipo);
        Assert.Equal("presupuesto_ya_respondido", resultado.Error.Codigo);
        AssertRespuesta(version, true, Ahora.AddHours(-1), otroUsuario);
        AssertSinCambios(EstadosDeReparacion.EsperandoAprobacion);
    }

    [Fact]
    public async Task Aprobar_registra_la_respuesta_en_la_vigente_pasa_a_aprobado_y_audita_ambas_cosas()
    {
        LlevarAEsperandoAprobacion();
        var anterior = AgregarVersion(1);
        var vigente = AgregarVersion(2);

        var resultado = await Crear().HandleAsync(new ResponderPresupuestoCommand(_orden.Id, true), default);

        Assert.True(resultado.EsExitoso);
        Assert.Equal(EstadosDeReparacion.Aprobado, resultado.Valor!.Estado);
        Assert.Equal(EstadosDeReparacion.Aprobado, _orden.Estado);
        Assert.Equal(1, _ordenes.Guardados);

        // Solo se responde la versión vigente (la de número más alto).
        AssertRespuesta(vigente, true, Ahora, UsuarioId);
        Assert.Null(anterior.RespuestaAprobada);

        var dto = resultado.Valor.PresupuestoVigente!;
        Assert.Equal(vigente.Id, dto.Id);
        Assert.True(dto.RespuestaAprobada);
        Assert.Equal(Ahora, dto.RespondidoEn);
        Assert.Equal("Juan", dto.RespondidoPor);

        Assert.Equal(2, _auditoria.Registros.Count);
        var respondido = _auditoria.Registros[0];
        Assert.Equal(AccionesAuditadas.PresupuestoRespondido, respondido.Accion);
        Assert.Equal(
            (EntidadesAuditadas.Reparacion, _orden.Id, UsuarioId, Ahora),
            (respondido.Entidad, respondido.EntidadId, respondido.UsuarioId, respondido.Fecha));
        Assert.Equal(
            [new CambioDeCampo("version_presupuesto", null, "2"), new CambioDeCampo("respuesta", null, "aprobado")],
            respondido.Cambios);

        var estado = _auditoria.Registros[1];
        Assert.Equal(AccionesAuditadas.EstadoCambiado, estado.Accion);
        Assert.Equal(
            (EntidadesAuditadas.Reparacion, _orden.Id, UsuarioId, Ahora),
            (estado.Entidad, estado.EntidadId, estado.UsuarioId, estado.Fecha));
        Assert.Equal(
            [new CambioDeCampo("estado", EstadosDeReparacion.EsperandoAprobacion, EstadosDeReparacion.Aprobado)],
            estado.Cambios);
    }

    [Fact]
    public async Task Rechazar_registra_la_respuesta_y_pasa_a_presupuesto_rechazado()
    {
        LlevarAEsperandoAprobacion();
        var vigente = AgregarVersion(1);

        var resultado = await Crear().HandleAsync(new ResponderPresupuestoCommand(_orden.Id, false), default);

        Assert.True(resultado.EsExitoso);
        Assert.Equal(EstadosDeReparacion.PresupuestoRechazado, resultado.Valor!.Estado);
        AssertRespuesta(vigente, false, Ahora, UsuarioId);
        Assert.False(resultado.Valor.PresupuestoVigente!.RespuestaAprobada);
        Assert.Equal(1, _ordenes.Guardados);

        Assert.Equal(
            [AccionesAuditadas.PresupuestoRespondido, AccionesAuditadas.EstadoCambiado],
            _auditoria.Registros.Select(r => r.Accion));
        Assert.Equal(
            [new CambioDeCampo("version_presupuesto", null, "1"), new CambioDeCampo("respuesta", null, "rechazado")],
            _auditoria.Registros[0].Cambios);
        Assert.Equal(
            [new CambioDeCampo("estado", EstadosDeReparacion.EsperandoAprobacion, EstadosDeReparacion.PresupuestoRechazado)],
            _auditoria.Registros[1].Cambios);
    }

    [Fact]
    public async Task Una_version_nueva_se_puede_responder_aunque_la_anterior_ya_tenga_respuesta()
    {
        LlevarAEsperandoAprobacion();
        AgregarVersion(1).RegistrarRespuesta(true, UsuarioId, Ahora.AddDays(-1));
        var nueva = AgregarVersion(2);

        var resultado = await Crear().HandleAsync(new ResponderPresupuestoCommand(_orden.Id, true), default);

        Assert.True(resultado.EsExitoso);
        Assert.True(nueva.RespuestaAprobada);
        Assert.Equal(EstadosDeReparacion.Aprobado, _orden.Estado);
    }
}
