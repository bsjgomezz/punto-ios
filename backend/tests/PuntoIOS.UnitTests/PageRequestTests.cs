using PuntoIOS.Application.Common.Pagination;

namespace PuntoIOS.UnitTests;

public class PageRequestTests
{
    [Fact]
    public void Usa_valores_por_defecto()
    {
        var request = new PageRequest();

        Assert.Equal(1, request.Page);
        Assert.Equal(20, request.PageSize);
        Assert.Equal(0, request.Skip);
    }

    [Theory]
    [InlineData(0, 1)]
    [InlineData(-5, 1)]
    [InlineData(3, 3)]
    public void Normaliza_la_pagina_minima(int page, int esperado)
    {
        Assert.Equal(esperado, new PageRequest(page, 10).Page);
    }

    [Theory]
    [InlineData(0, 20)]
    [InlineData(-1, 20)]
    [InlineData(50, 50)]
    [InlineData(100, 100)]
    [InlineData(101, 100)]
    [InlineData(100000, 100)]
    public void Limita_el_tamano_de_pagina(int pageSize, int esperado)
    {
        Assert.Equal(esperado, new PageRequest(1, pageSize).PageSize);
    }

    [Fact]
    public void Calcula_los_registros_a_omitir()
    {
        Assert.Equal(40, new PageRequest(3, 20).Skip);
    }

    [Fact]
    public void No_desborda_con_paginas_enormes()
    {
        Assert.Equal(int.MaxValue, new PageRequest(int.MaxValue, 100).Skip);
    }
}
