using PuntoIOS.Domain.Reparaciones;

namespace PuntoIOS.UnitTests.Reparaciones;

public class MaquinaDeEstadosTests
{
    private static readonly DateTimeOffset Ahora = new(2026, 11, 8, 12, 0, 0, TimeSpan.Zero);

    private static OrdenReparacion Orden(string? desbloqueoTipo = null, string? desbloqueoCifrado = null) =>
        OrdenReparacion.Recibir(
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "No carga", [TiposDeReparacion.ConectorDeCarga], null, null, null,
            desbloqueoTipo, desbloqueoCifrado, null, Ahora);

    [Fact]
    public void Hay_quince_estados_y_todos_tienen_sus_transiciones_definidas()
    {
        Assert.Equal(15, EstadosDeReparacion.Catalogo.Count);
        Assert.Equal(
            EstadosDeReparacion.Catalogo.Keys.Order(StringComparer.Ordinal),
            MaquinaDeEstados.Transiciones.Keys.Order(StringComparer.Ordinal));
        Assert.All(
            MaquinaDeEstados.Transiciones.Values.SelectMany(destinos => destinos),
            destino => Assert.True(EstadosDeReparacion.Catalogo.ContainsKey(destino)));
    }

    [Theory]
    [InlineData("recibido", "en_pruebas")]
    [InlineData("en_diagnostico", "irreparable")]
    [InlineData("esperando_aprobacion", "presupuesto_rechazado")]
    [InlineData("presupuesto_rechazado", "entregado")]
    [InlineData("en_reparacion", "esperando_aprobacion")]
    [InlineData("reparacion_completada", "en_reparacion")]
    [InlineData("listo_para_retirar", "entregado")]
    public void Permite_las_transiciones_de_la_tabla(string desde, string hacia) =>
        Assert.True(MaquinaDeEstados.PuedeTransicionar(desde, hacia));

    [Theory]
    [InlineData("recibido", "entregado")]
    [InlineData("recibido", "recibido")]
    [InlineData("presupuesto_enviado", "cancelado")]
    [InlineData("esperando_aprobacion", "cancelado")]
    [InlineData("entregado", "recibido")]
    [InlineData("entregado", "en_reparacion")]
    [InlineData("cancelado", "recibido")]
    [InlineData("cancelado", "entregado")]
    public void Rechaza_lo_que_no_esta_en_la_tabla_y_cualquier_salida_de_un_estado_terminal(string desde, string hacia) =>
        Assert.False(MaquinaDeEstados.PuedeTransicionar(desde, hacia));

    [Fact]
    public void Un_estado_inexistente_no_permite_nada_y_no_tira_excepcion()
    {
        Assert.False(MaquinaDeEstados.PuedeTransicionar("teletransportado", "en_pruebas"));
        Assert.False(MaquinaDeEstados.PuedeTransicionar("recibido", "teletransportado"));
        Assert.Empty(MaquinaDeEstados.SiguientesEstados("teletransportado"));
    }

    [Fact]
    public void Los_estados_terminales_no_tienen_siguientes()
    {
        Assert.Empty(MaquinaDeEstados.SiguientesEstados(EstadosDeReparacion.Entregado));
        Assert.Empty(MaquinaDeEstados.SiguientesEstados(EstadosDeReparacion.Cancelado));
        Assert.Equal(
            [EstadosDeReparacion.EnPruebas, EstadosDeReparacion.Cancelado],
            MaquinaDeEstados.SiguientesEstados(EstadosDeReparacion.Recibido));
    }

    [Fact]
    public void Cambiar_a_un_estado_que_no_existe_tira_excepcion()
    {
        var orden = Orden();

        Assert.Throws<ArgumentException>(() => orden.CambiarEstado("teletransportado"));
        Assert.Equal(EstadosDeReparacion.Recibido, orden.Estado);
    }

    [Fact]
    public void Una_transicion_no_permitida_tira_excepcion_y_no_cambia_el_estado()
    {
        var orden = Orden();

        Assert.Throws<ArgumentException>(() => orden.CambiarEstado(EstadosDeReparacion.Entregado));
        Assert.Throws<ArgumentException>(() => orden.CambiarEstado(EstadosDeReparacion.Recibido));
        Assert.Equal(EstadosDeReparacion.Recibido, orden.Estado);
    }

    [Fact]
    public void Una_transicion_permitida_cambia_el_estado()
    {
        var orden = Orden();

        orden.CambiarEstado(EstadosDeReparacion.EnPruebas);

        Assert.Equal(EstadosDeReparacion.EnPruebas, orden.Estado);
    }

    [Fact]
    public void Al_entregar_el_equipo_se_borra_el_desbloqueo()
    {
        var orden = Orden(TiposDeDesbloqueo.Pin, "cifrado");

        orden.CambiarEstado(EstadosDeReparacion.EnPruebas);
        orden.CambiarEstado(EstadosDeReparacion.EnDiagnostico);
        orden.CambiarEstado(EstadosDeReparacion.EsperandoPresupuesto);
        orden.CambiarEstado(EstadosDeReparacion.PresupuestoEnviado);
        orden.CambiarEstado(EstadosDeReparacion.EsperandoAprobacion);
        orden.CambiarEstado(EstadosDeReparacion.PresupuestoRechazado);
        Assert.True(orden.TieneDesbloqueo);

        orden.CambiarEstado(EstadosDeReparacion.Entregado);

        Assert.Equal(EstadosDeReparacion.Entregado, orden.Estado);
        Assert.False(orden.TieneDesbloqueo);
        Assert.Null(orden.DesbloqueoTipo);
        Assert.Null(orden.DesbloqueoCifrado);
    }

    [Fact]
    public void Cancelar_no_borra_el_desbloqueo()
    {
        var orden = Orden(TiposDeDesbloqueo.Pin, "cifrado");

        orden.CambiarEstado(EstadosDeReparacion.Cancelado);

        Assert.True(orden.TieneDesbloqueo);
    }
}
