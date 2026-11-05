using PuntoIOS.Application.Common.Pagination;

namespace PuntoIOS.UnitTests;

public class PagedResultTests
{
    [Theory]
    [InlineData(0, 20, 0)]
    [InlineData(1, 20, 1)]
    [InlineData(20, 20, 1)]
    [InlineData(21, 20, 2)]
    [InlineData(100, 20, 5)]
    public void Calcula_el_total_de_paginas(int totalCount, int pageSize, int esperado)
    {
        var result = new PagedResult<string>([], 1, pageSize, totalCount);

        Assert.Equal(esperado, result.TotalPages);
    }
}
