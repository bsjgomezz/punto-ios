using PuntoIOS.Domain.Auditoria;
using PuntoIOS.Domain.Clientes;

namespace PuntoIOS.UnitTests.Clientes;

public class DispositivoTests
{
    private static readonly DateTimeOffset Ahora = new(2026, 11, 7, 12, 0, 0, TimeSpan.Zero);
    private const string ImeiValido = "490154203237518";

    private static Dispositivo Nuevo(string? imei = ImeiValido) =>
        Dispositivo.Registrar(Guid.NewGuid(), Guid.NewGuid(), "Apple", "iPhone 13", "128 GB", "Azul", imei, null, null, Ahora);

    [Theory]
    [InlineData("49 0154 2032 3751-8", "490154203237518")]
    [InlineData("  f2lxk0abcd12 ", "F2LXK0ABCD12")]
    [InlineData("   ", null)]
    [InlineData(null, null)]
    public void Normaliza_el_imei_dejando_letras_y_digitos_en_mayusculas(string? entrada, string? esperado) =>
        Assert.Equal(esperado, Dispositivo.NormalizarImei(entrada));

    [Theory]
    [InlineData("490154203237518", true)]
    [InlineData("356938035643809", true)]
    [InlineData("490154203237519", false)]
    [InlineData("F2LXK0ABCD12", true)]
    [InlineData("1234567", false)]
    [InlineData("123456789012345678901", false)]
    public void Valida_el_digito_verificador_del_imei_y_el_largo_del_numero_de_serie(string imei, bool valido) =>
        Assert.Equal(valido, Dispositivo.ImeiEsValido(imei));

    [Fact]
    public void Enmascara_dejando_visibles_los_ultimos_cuatro()
    {
        Assert.Equal("•••••••••••7518", Nuevo().ImeiSerieEnmascarado);
        Assert.Null(Nuevo(imei: null).ImeiSerieEnmascarado);
    }

    [Fact]
    public void Rechaza_un_imei_con_digito_verificador_incorrecto()
    {
        Assert.Throws<ArgumentException>(() => Nuevo("490154203237519"));
    }

    [Fact]
    public void Editar_sin_tocar_el_imei_lo_conserva_y_registra_solo_lo_que_cambio()
    {
        var dispositivo = Nuevo();

        var cambios = dispositivo.Actualizar("Apple", "iPhone 13 Pro", "128 GB", "", null, actualizarImei: false, imeiSerie: null);

        Assert.Equal(new[] { "modelo", "color" }, cambios.Select(c => c.Campo));
        Assert.Equal(ImeiValido, dispositivo.ImeiSerie);
        Assert.Null(dispositivo.Color);
    }

    [Fact]
    public void Cambiar_el_imei_deja_el_historial_enmascarado()
    {
        var dispositivo = Nuevo();

        var cambios = dispositivo.Actualizar("Apple", "iPhone 13", "128 GB", "Azul", null, actualizarImei: true, imeiSerie: "356938035643809");

        var cambio = Assert.Single(cambios);
        Assert.Equal(new CambioDeCampo("imei_serie", "•••••••••••7518", "•••••••••••3809"), cambio);
        Assert.Equal("356938035643809", dispositivo.ImeiSerie);
    }
}
