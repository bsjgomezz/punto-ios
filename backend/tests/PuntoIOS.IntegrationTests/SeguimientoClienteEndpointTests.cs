using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PuntoIOS.Api.Controllers;
using PuntoIOS.Application.Common.Pagination;
using PuntoIOS.Application.Features.Auditoria;
using PuntoIOS.Application.Features.Clientes;
using PuntoIOS.Application.Features.Clientes.SeguimientoCliente;
using PuntoIOS.Domain.Clientes;
using PuntoIOS.Domain.Identidad;
using PuntoIOS.Infrastructure.Persistence;

namespace PuntoIOS.IntegrationTests;

public class SeguimientoClienteEndpointTests(PostgresFixture postgres) : IClassFixture<PostgresFixture>
{
    private static async Task<ClienteDto> CrearClienteAsync(HttpClient http) =>
        (await (await http.PostAsJsonAsync("/api/v1/clientes", new CrearClienteRequest("Ana", "Pérez", "3498 111111", null, null, null)))
            .Content.ReadFromJsonAsync<ClienteDto>())!;

    [Fact]
    public async Task Genera_consulta_y_regenera_el_codigo_sin_guardarlo_en_texto_plano()
    {
        using var api = await TestApi.CrearAsync(postgres);
        var admin = api.ClienteAdministrador();
        var ana = await CrearClienteAsync(admin);
        var ruta = $"/api/v1/clientes/{ana.Id}/seguimiento";
        Assert.Null(ana.CodigoSeguimientoGeneradoEn);

        var generado = await (await admin.PostAsync(ruta, null)).Content.ReadFromJsonAsync<CodigoSeguimientoDto>();
        Assert.True(CodigoSeguimiento.EsValido(generado!.Codigo));

        var consultado = await (await admin.PostAsync($"{ruta}/consultar", null)).Content.ReadFromJsonAsync<CodigoSeguimientoDto>();
        Assert.Equal(generado.Codigo, consultado!.Codigo);
        Assert.NotNull((await admin.GetFromJsonAsync<ClienteDto>($"/api/v1/clientes/{ana.Id}"))!.CodigoSeguimientoGeneradoEn);

        await using (var scope = api.Servicios.CreateAsyncScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<PuntoIosDbContext>();
            var guardado = await db.Clientes.AsNoTracking().SingleAsync(c => c.Id == ana.Id);
            Assert.Equal(CodigoSeguimiento.Huella(generado.Codigo), guardado.CodigoSeguimientoHuella);
            Assert.DoesNotContain(generado.Codigo, guardado.CodigoSeguimientoCifrado!);
        }

        var regenerado = await (await admin.PostAsync(ruta, null)).Content.ReadFromJsonAsync<CodigoSeguimientoDto>();
        Assert.NotEqual(generado.Codigo, regenerado!.Codigo);

        var historial = await admin.GetFromJsonAsync<PagedResult<RegistroHistorialDto>>($"/api/v1/clientes/{ana.Id}/historial");
        Assert.Equal(
            new[] { "codigo_regenerado", "codigo_consultado", "codigo_generado", "creado" },
            historial!.Items.Select(h => h.Accion));
        Assert.DoesNotContain(generado.Codigo, await admin.GetStringAsync($"/api/v1/clientes/{ana.Id}/historial"));
    }

    [Fact]
    public async Task Consultar_sin_codigo_o_de_un_cliente_inexistente_responde_404()
    {
        using var api = await TestApi.CrearAsync(postgres);
        var admin = api.ClienteAdministrador();
        var ana = await CrearClienteAsync(admin);

        Assert.Equal(HttpStatusCode.NotFound, (await admin.PostAsync($"/api/v1/clientes/{ana.Id}/seguimiento/consultar", null)).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await admin.PostAsync($"/api/v1/clientes/{Guid.NewGuid()}/seguimiento", null)).StatusCode);
    }

    [Fact]
    public async Task Matriz_de_autorizacion_del_codigo_de_seguimiento()
    {
        using var api = await TestApi.CrearAsync(postgres);
        var ana = await CrearClienteAsync(api.ClienteAdministrador());
        var recepcion = api.Cliente(await api.RegistrarUsuarioAsync("recepcion@x.com", RolesDeSistema.RecepcionVentasId));
        var tecnico = api.Cliente(await api.RegistrarUsuarioAsync("tecnico@x.com", RolesDeSistema.TecnicoId));
        var anonimo = api.Cliente();
        var ruta = $"/api/v1/clientes/{ana.Id}/seguimiento";

        Assert.Equal(HttpStatusCode.OK, (await recepcion.PostAsync(ruta, null)).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await recepcion.PostAsync($"{ruta}/consultar", null)).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await tecnico.PostAsync(ruta, null)).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await tecnico.PostAsync($"{ruta}/consultar", null)).StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, (await anonimo.PostAsync($"{ruta}/consultar", null)).StatusCode);
    }
}
