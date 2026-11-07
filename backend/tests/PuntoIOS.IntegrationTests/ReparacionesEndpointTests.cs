using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PuntoIOS.Api.Controllers;
using PuntoIOS.Application.Common.Pagination;
using PuntoIOS.Application.Features.Auditoria;
using PuntoIOS.Application.Features.Clientes;
using PuntoIOS.Application.Features.Dispositivos;
using PuntoIOS.Application.Features.Identidad.Puertos;
using PuntoIOS.Application.Features.Reparaciones;
using PuntoIOS.Application.Features.Reparaciones.VerDesbloqueo;
using PuntoIOS.Domain.Identidad;
using PuntoIOS.Infrastructure.Persistence;

namespace PuntoIOS.IntegrationTests;

public class ReparacionesEndpointTests(PostgresFixture postgres) : IClassFixture<PostgresFixture>
{
    private const string Ruta = "/api/v1/reparaciones";

    private static async Task<(ClienteDto Cliente, DispositivoDto Equipo)> ClienteConEquipoAsync(HttpClient admin, string telefono = "3498 111111")
    {
        var cliente = (await (await admin.PostAsJsonAsync("/api/v1/clientes", new CrearClienteRequest("Ana", "Pérez", telefono, null, null, null)))
            .Content.ReadFromJsonAsync<ClienteDto>())!;
        var equipo = (await (await admin.PostAsJsonAsync(
                $"/api/v1/clientes/{cliente.Id}/dispositivos", new CrearDispositivoRequest("Apple", "iPhone 13", null, null, null, null)))
            .Content.ReadFromJsonAsync<DispositivoDto>())!;
        return (cliente, equipo);
    }

    private static RecibirEquipoRequest Orden(Guid clienteId, Guid? dispositivoId, EquipoNuevoRequest? nuevo = null, Guid? tecnico = null,
        string? desbloqueoTipo = null, string? desbloqueoValor = null) =>
        new(clienteId, dispositivoId, nuevo, "No carga", ["conector_carga"], null, tecnico, null, desbloqueoTipo, desbloqueoValor);

    [Fact]
    public async Task Las_ordenes_se_numeran_de_a_una_y_se_listan_de_la_mas_nueva_a_la_mas_vieja()
    {
        using var api = await TestApi.CrearAsync(postgres);
        var admin = api.ClienteAdministrador();
        var (ana, iphone) = await ClienteConEquipoAsync(admin);

        var primera = await admin.PostAsJsonAsync(Ruta, Orden(ana.Id, iphone.Id));
        var segunda = await admin.PostAsJsonAsync(
            Ruta, Orden(ana.Id, null, new EquipoNuevoRequest("Apple", "iPhone 15", "256 GB", null, null)));

        Assert.Equal(HttpStatusCode.Created, primera.StatusCode);
        Assert.Equal(1, (await primera.Content.ReadFromJsonAsync<OrdenReparacionDto>())!.Numero);
        var dto = (await segunda.Content.ReadFromJsonAsync<OrdenReparacionDto>())!;
        Assert.Equal(2, dto.Numero);
        Assert.Equal("recibido", dto.Estado);
        Assert.Equal("iPhone 15", dto.Equipo.Modelo);

        var delCliente = await admin.GetFromJsonAsync<PagedResult<OrdenResumenDto>>($"{Ruta}?clienteId={ana.Id}");
        Assert.Equal(new long[] { 2, 1 }, delCliente!.Items.Select(o => o.Numero));
        Assert.Equal("Pérez, Ana", delCliente.Items[0].Cliente);
        Assert.Equal(2, (await admin.GetFromJsonAsync<List<DispositivoDto>>($"/api/v1/clientes/{ana.Id}/dispositivos"))!.Count);
    }

    [Fact]
    public async Task El_desbloqueo_se_guarda_cifrado_y_solo_lo_ven_quienes_reparan()
    {
        using var api = await TestApi.CrearAsync(postgres);
        var admin = api.ClienteAdministrador();
        var (ana, iphone) = await ClienteConEquipoAsync(admin);
        var recepcion = api.Cliente(await api.RegistrarUsuarioAsync("recepcion@x.com", RolesDeSistema.RecepcionVentasId));
        var tecnico = api.Cliente(await api.RegistrarUsuarioAsync("tecnico@x.com", RolesDeSistema.TecnicoId));

        var creada = await recepcion.PostAsJsonAsync(Ruta, Orden(ana.Id, iphone.Id, desbloqueoTipo: "pin", desbloqueoValor: "246810"));
        Assert.Equal(HttpStatusCode.Created, creada.StatusCode);
        var orden = (await creada.Content.ReadFromJsonAsync<OrdenReparacionDto>())!;
        Assert.Equal("pin", orden.DesbloqueoTipo);
        Assert.DoesNotContain("246810", await admin.GetStringAsync($"{Ruta}/{orden.Id}"));

        await using (var scope = api.Servicios.CreateAsyncScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<PuntoIosDbContext>();
            var guardada = await db.OrdenesReparacion.AsNoTracking().SingleAsync(o => o.Id == orden.Id);
            Assert.DoesNotContain("246810", guardada.DesbloqueoCifrado!);
        }

        Assert.Equal(HttpStatusCode.Forbidden, (await recepcion.PostAsync($"{Ruta}/{orden.Id}/desbloqueo", null)).StatusCode);
        var visto = await tecnico.PostAsync($"{Ruta}/{orden.Id}/desbloqueo", null);
        Assert.Equal(HttpStatusCode.OK, visto.StatusCode);
        Assert.Equal(new DesbloqueoDto("pin", "246810"), await visto.Content.ReadFromJsonAsync<DesbloqueoDto>());

        var historial = await admin.GetFromJsonAsync<PagedResult<RegistroHistorialDto>>($"{Ruta}/{orden.Id}/historial");
        Assert.Equal(new[] { "desbloqueo_consultado", "creado" }, historial!.Items.Select(h => h.Accion));
    }

    [Fact]
    public async Task Solo_se_puede_asignar_a_quien_repara()
    {
        using var api = await TestApi.CrearAsync(postgres);
        var admin = api.ClienteAdministrador();
        var (ana, iphone) = await ClienteConEquipoAsync(admin);
        await api.RegistrarUsuarioAsync("tecnico@x.com", RolesDeSistema.TecnicoId);
        await api.RegistrarUsuarioAsync("recepcion@x.com", RolesDeSistema.RecepcionVentasId);
        await api.RegistrarUsuarioAsync("inactivo@x.com", RolesDeSistema.TecnicoId, activo: false);

        var tecnicos = (await admin.GetFromJsonAsync<List<UsuarioResumen>>($"{Ruta}/tecnicos"))!;
        Assert.Equal(2, tecnicos.Count); // el técnico activo y el administrador

        var tecnicoId = tecnicos.First(t => t.Nombre.Contains("tecnico", StringComparison.OrdinalIgnoreCase)).Id;
        var asignada = await admin.PostAsJsonAsync(Ruta, Orden(ana.Id, iphone.Id, tecnico: tecnicoId));
        Assert.Equal(HttpStatusCode.Created, asignada.StatusCode);
        Assert.Equal(tecnicoId, (await asignada.Content.ReadFromJsonAsync<OrdenReparacionDto>())!.Tecnico!.Id);

        var invalida = await admin.PostAsJsonAsync(Ruta, Orden(ana.Id, iphone.Id, tecnico: Guid.NewGuid()));
        var problema = await invalida.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal("tecnico_invalido", problema.GetProperty("codigo").GetString());
    }

    [Fact]
    public async Task Validaciones_e_inexistentes()
    {
        using var api = await TestApi.CrearAsync(postgres);
        var admin = api.ClienteAdministrador();
        var (ana, iphone) = await ClienteConEquipoAsync(admin);
        var (luis, iphoneDeLuis) = await ClienteConEquipoAsync(admin, "3498 222222");

        var sinTipos = await admin.PostAsJsonAsync(Ruta, Orden(ana.Id, iphone.Id) with { TiposReparacion = [] });
        var equipoAjeno = await admin.PostAsJsonAsync(Ruta, Orden(ana.Id, iphoneDeLuis.Id));
        var clienteInexistente = await admin.PostAsJsonAsync(Ruta, Orden(Guid.NewGuid(), iphone.Id));

        Assert.Equal(HttpStatusCode.BadRequest, sinTipos.StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, equipoAjeno.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, clienteInexistente.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await admin.GetAsync($"{Ruta}/{Guid.NewGuid()}")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await admin.PostAsync($"{Ruta}/{Guid.NewGuid()}/desbloqueo", null)).StatusCode);
        Assert.Empty((await admin.GetFromJsonAsync<PagedResult<OrdenResumenDto>>($"{Ruta}?clienteId={luis.Id}"))!.Items);
    }

    [Fact]
    public async Task Matriz_de_autorizacion_de_reparaciones()
    {
        using var api = await TestApi.CrearAsync(postgres);
        var admin = api.ClienteAdministrador();
        var (ana, iphone) = await ClienteConEquipoAsync(admin);
        var tecnico = api.Cliente(await api.RegistrarUsuarioAsync("tecnico@x.com", RolesDeSistema.TecnicoId));
        var anonimo = api.Cliente();

        Assert.Equal(HttpStatusCode.OK, (await tecnico.GetAsync(Ruta)).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await tecnico.GetAsync($"{Ruta}/tipos")).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await tecnico.PostAsJsonAsync(Ruta, Orden(ana.Id, iphone.Id))).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await tecnico.GetAsync($"{Ruta}/tecnicos")).StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, (await anonimo.GetAsync(Ruta)).StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, (await anonimo.PostAsJsonAsync(Ruta, Orden(ana.Id, iphone.Id))).StatusCode);
    }
}
