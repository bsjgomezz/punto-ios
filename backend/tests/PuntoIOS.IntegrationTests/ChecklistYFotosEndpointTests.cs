using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using PuntoIOS.Api.Controllers;
using PuntoIOS.Application.Features.Clientes;
using PuntoIOS.Application.Features.Dispositivos;
using PuntoIOS.Application.Features.Reparaciones;
using PuntoIOS.Domain.Reparaciones;

namespace PuntoIOS.IntegrationTests;

public class ChecklistYFotosEndpointTests(PostgresFixture postgres) : IClassFixture<PostgresFixture>
{
    private const string Ruta = "/api/v1/reparaciones";

    private static readonly byte[] Jpeg =
        [0xFF, 0xD8, 0xFF, 0xE0, 0, 0x10, (byte)'J', (byte)'F', (byte)'I', (byte)'F', 0, 1, 1, 2, 3];

    private static async Task<(ClienteDto Cliente, DispositivoDto Equipo)> ClienteConEquipoAsync(HttpClient admin)
    {
        var cliente = (await (await admin.PostAsJsonAsync("/api/v1/clientes", new CrearClienteRequest("Ana", "Pérez", "3498 111111", null, null, null)))
            .Content.ReadFromJsonAsync<ClienteDto>())!;
        var equipo = (await (await admin.PostAsJsonAsync(
                $"/api/v1/clientes/{cliente.Id}/dispositivos", new CrearDispositivoRequest("Apple", "iPhone 13", null, null, null, null)))
            .Content.ReadFromJsonAsync<DispositivoDto>())!;
        return (cliente, equipo);
    }

    private static RecibirEquipoRequest Orden(Guid clienteId, Guid dispositivoId, ChecklistRequest? checklist = null, DateOnly? fecha = null) =>
        new(clienteId, dispositivoId, null, "No carga", ["conector_carga"], null, null, fecha, null, null, checklist);

    [Fact]
    public async Task El_checklist_viaja_completo_y_solo_los_items_revisados()
    {
        using var api = await TestApi.CrearAsync(postgres);
        var admin = api.ClienteAdministrador();
        var (ana, iphone) = await ClienteConEquipoAsync(admin);
        var checklist = new ChecklistRequest(
            new Dictionary<string, bool> { ["enciende"] = true, ["face_id"] = false }, 87, "Vidrio trasero astillado");

        var creada = await admin.PostAsJsonAsync(Ruta, Orden(ana.Id, iphone.Id, checklist));

        Assert.Equal(HttpStatusCode.Created, creada.StatusCode);
        var orden = (await creada.Content.ReadFromJsonAsync<OrdenReparacionDto>())!;
        Assert.Equal(checklist.Items, orden.Checklist.Items);
        Assert.Equal(87, orden.Checklist.SaludBateria);
        Assert.Equal("Vidrio trasero astillado", orden.Checklist.Notas);
        Assert.False(orden.NoEnciende);

        var releida = await admin.GetFromJsonAsync<OrdenReparacionDto>($"{Ruta}/{orden.Id}");
        Assert.Equal(checklist.Items, releida!.Checklist.Items);
    }

    [Fact]
    public async Task Si_no_enciende_la_api_rechaza_la_fecha_estimada_con_el_motivo()
    {
        using var api = await TestApi.CrearAsync(postgres);
        var admin = api.ClienteAdministrador();
        var (ana, iphone) = await ClienteConEquipoAsync(admin);
        var noEnciende = new ChecklistRequest(new Dictionary<string, bool> { ["enciende"] = false }, null, null);

        var rechazada = await admin.PostAsJsonAsync(Ruta, Orden(ana.Id, iphone.Id, noEnciende, new DateOnly(2026, 11, 20)));
        var aceptada = await admin.PostAsJsonAsync(Ruta, Orden(ana.Id, iphone.Id, noEnciende));

        Assert.Equal(HttpStatusCode.BadRequest, rechazada.StatusCode);
        var problema = await rechazada.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal("fecha_no_determinable", problema.GetProperty("codigo").GetString());
        Assert.Contains(ItemsDeChecklist.RazonSinFecha, problema.GetProperty("title").GetString());

        Assert.Equal(HttpStatusCode.Created, aceptada.StatusCode);
        var orden = (await aceptada.Content.ReadFromJsonAsync<OrdenReparacionDto>())!;
        Assert.True(orden.NoEnciende);
        Assert.Null(orden.FechaEstimadaEntrega);
    }

    [Fact]
    public async Task Sube_lista_y_descarga_una_foto_de_la_orden()
    {
        using var api = await TestApi.CrearAsync(postgres);
        var admin = api.ClienteAdministrador();
        var (ana, iphone) = await ClienteConEquipoAsync(admin);
        var orden = (await (await admin.PostAsJsonAsync(Ruta, Orden(ana.Id, iphone.Id))).Content.ReadFromJsonAsync<OrdenReparacionDto>())!;

        using var contenido = new MultipartFormDataContent();
        var archivo = new ByteArrayContent(Jpeg);
        archivo.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
        contenido.Add(archivo, "foto", "celular.jpg");

        var subida = await admin.PostAsync($"{Ruta}/{orden.Id}/fotos", contenido);
        Assert.Equal(HttpStatusCode.OK, subida.StatusCode);
        var fotoSubida = (await subida.Content.ReadFromJsonAsync<FotoDto>())!;

        var listado = await admin.GetFromJsonAsync<List<FotoDto>>($"{Ruta}/{orden.Id}/fotos");
        Assert.Equal(new[] { fotoSubida.Id }, listado!.Select(f => f.Id));

        var descarga = await admin.GetAsync($"{Ruta}/{orden.Id}/fotos/{fotoSubida.Id}");
        Assert.Equal(HttpStatusCode.OK, descarga.StatusCode);
        Assert.Equal("image/jpeg", descarga.Content.Headers.ContentType!.MediaType);
        Assert.Equal(Jpeg, await descarga.Content.ReadAsByteArrayAsync());

        Assert.Equal(HttpStatusCode.NotFound, (await admin.GetAsync($"{Ruta}/{orden.Id}/fotos/{Guid.NewGuid()}")).StatusCode);
    }

    [Fact]
    public async Task Rechaza_archivos_que_no_son_imagenes_y_sin_adjuntar_nada()
    {
        using var api = await TestApi.CrearAsync(postgres);
        var admin = api.ClienteAdministrador();
        var (ana, iphone) = await ClienteConEquipoAsync(admin);
        var orden = (await (await admin.PostAsJsonAsync(Ruta, Orden(ana.Id, iphone.Id))).Content.ReadFromJsonAsync<OrdenReparacionDto>())!;

        using var contenidoPdf = new MultipartFormDataContent();
        var pdf = new ByteArrayContent("%PDF-1.7 contenido"u8.ToArray());
        pdf.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/pdf");
        contenidoPdf.Add(pdf, "foto", "informe.pdf");
        var noImagen = await admin.PostAsync($"{Ruta}/{orden.Id}/fotos", contenidoPdf);

        using var vacio = new MultipartFormDataContent();
        vacio.Add(new StringContent("algo"), "otroCampo");
        var sinArchivo = await admin.PostAsync($"{Ruta}/{orden.Id}/fotos", vacio);

        Assert.Equal(HttpStatusCode.BadRequest, noImagen.StatusCode);
        Assert.Equal("formato_no_admitido", (await noImagen.Content.ReadFromJsonAsync<JsonElement>()).GetProperty("codigo").GetString());
        Assert.Equal(HttpStatusCode.BadRequest, sinArchivo.StatusCode);
        Assert.Equal("foto_obligatoria", (await sinArchivo.Content.ReadFromJsonAsync<JsonElement>()).GetProperty("codigo").GetString());
    }
}
