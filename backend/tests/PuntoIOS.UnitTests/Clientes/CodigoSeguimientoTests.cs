using PuntoIOS.Domain.Clientes;

namespace PuntoIOS.UnitTests.Clientes;

public class CodigoSeguimientoTests
{
    private const string Alfabeto = "0123456789ABCDEFGHJKMNPQRSTVWXYZ";

    [Fact]
    public void Genera_codigos_validos_del_largo_esperado_y_distintos()
    {
        var codigos = Enumerable.Range(0, 500).Select(_ => CodigoSeguimiento.Generar()).ToList();

        Assert.All(codigos, c =>
        {
            Assert.Equal(CodigoSeguimiento.Largo, c.Length);
            Assert.True(CodigoSeguimiento.EsValido(c));
        });
        Assert.Equal(codigos.Count, codigos.Distinct().Count());
    }

    [Fact]
    public void Detecta_cualquier_caracter_cambiado()
    {
        var codigo = CodigoSeguimiento.Generar();

        for (var i = 0; i < codigo.Length; i++)
        {
            foreach (var otro in Alfabeto.Where(c => c != codigo[i]))
            {
                var alterado = codigo[..i] + otro + codigo[(i + 1)..];
                Assert.False(CodigoSeguimiento.EsValido(alterado), $"No detectó el cambio en la posición {i}");
            }
        }
    }

    [Fact]
    public void Detecta_dos_caracteres_vecinos_invertidos()
    {
        var codigo = CodigoSeguimiento.Generar();

        for (var i = 0; i < 25; i++)
        {
            if (codigo[i] == codigo[i + 1])
            {
                continue;
            }

            var invertido = codigo[..i] + codigo[i + 1] + codigo[i] + codigo[(i + 2)..];
            Assert.False(CodigoSeguimiento.EsValido(invertido));
        }
    }

    [Fact]
    public void Normaliza_minusculas_guiones_y_letras_que_se_confunden()
    {
        var codigo = CodigoSeguimiento.Generar();
        var tipeado = CodigoSeguimiento.Formatear(codigo).ToLowerInvariant().Replace('1', 'l').Replace('0', 'o');

        Assert.Equal(codigo, CodigoSeguimiento.Normalizar(tipeado));
        Assert.Null(CodigoSeguimiento.Normalizar("ABC@123"));
    }

    [Fact]
    public void La_huella_es_sha256_en_hexadecimal_y_no_contiene_el_codigo()
    {
        var codigo = CodigoSeguimiento.Generar();

        var huella = CodigoSeguimiento.Huella(codigo);

        Assert.Equal(64, huella.Length);
        Assert.Equal(huella, CodigoSeguimiento.Huella(codigo));
        Assert.DoesNotContain(codigo, huella, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Formatea_en_grupos_de_cuatro()
    {
        Assert.Equal("ABCD-EFGH-JK", CodigoSeguimiento.Formatear("ABCDEFGHJK"));
    }
}
