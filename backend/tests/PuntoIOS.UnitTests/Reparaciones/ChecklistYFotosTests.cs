using PuntoIOS.Application.Common.Archivos;
using PuntoIOS.Application.Features.Reparaciones.Fotos;
using PuntoIOS.Application.Features.Reparaciones.RecibirEquipo;
using PuntoIOS.Domain.Clientes;
using PuntoIOS.Domain.Reparaciones;
using PuntoIOS.UnitTests.Clientes;
using PuntoIOS.UnitTests.Identidad;

namespace PuntoIOS.UnitTests.Reparaciones;

public class ChecklistYFotosTests
{
    private static readonly DateTimeOffset Ahora = new(2026, 11, 7, 12, 0, 0, TimeSpan.Zero);
    private static readonly Guid LocalId = Guid.NewGuid();
    private static readonly Guid UsuarioId = Guid.NewGuid();

    internal static readonly byte[] Jpeg = [0xFF, 0xD8, 0xFF, 0xE0, 0, 0x10, (byte)'J', (byte)'F', (byte)'I', (byte)'F', 0, 1, 1, 2, 3];
    internal static readonly byte[] Png = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, 0, 0, 0, 13];
    internal static readonly byte[] Webp = [.."RIFF"u8.ToArray(), 0, 0, 0, 0, .."WEBP"u8.ToArray()];

    private readonly ClienteRepoEnMemoria _clientes = new();
    private readonly DispositivoRepoEnMemoria _dispositivos = new();
    private readonly AuditoriaRepoEnMemoria _auditoria = new();
    private readonly OrdenRepoEnMemoria _ordenes = new();
    private readonly AlmacenEnMemoria _almacen = new();
    private readonly Cliente _ana;
    private readonly Dispositivo _iphone;

    public ChecklistYFotosTests()
    {
        _ana = Cliente.Registrar(LocalId, "Ana", "Pérez", "3498 111111", null, null, null, null, Ahora);
        _clientes.Clientes.Add(_ana);
        _iphone = Dispositivo.Registrar(LocalId, _ana.Id, "Apple", "iPhone 13", null, null, null, null, null, Ahora);
        _dispositivos.Dispositivos.Add(_iphone);
    }

    private RecibirEquipoHandler Recibir() =>
        new(_ordenes, _clientes, _dispositivos, _auditoria, new DirectorioFijo([]), new CifradoDePrueba(),
            new LocalFijo(LocalId), new UsuarioFijo(UsuarioId), new RelojFijo(Ahora));

    private AgregarFotoHandler AgregarFoto() =>
        new(_ordenes, _almacen, _auditoria, new LocalFijo(LocalId), new UsuarioFijo(UsuarioId), new RelojFijo(Ahora));

    private RecibirEquipoCommand Comando(ChecklistIngreso? checklist, DateOnly? fecha = null) =>
        new(_ana.Id, _iphone.Id, null, "No carga", [TiposDeReparacion.ConectorDeCarga], null, null, fecha, null, null, checklist);

    private static ChecklistIngreso Checklist(params (string Codigo, bool Valor)[] items) =>
        new(items.Select(i => new ItemChecklist(i.Codigo, i.Valor)).ToList(), 87, "  Vidrio trasero astillado  ");

    [Fact]
    public async Task Guarda_solo_los_items_revisados_con_la_bateria_y_las_notas()
    {
        var resultado = await Recibir().HandleAsync(Comando(Checklist(("enciende", true), ("face_id", false))), default);

        Assert.True(resultado.EsExitoso);
        var checklist = resultado.Valor!.Checklist;
        Assert.Equal(new Dictionary<string, bool> { ["enciende"] = true, ["face_id"] = false }, checklist.Items);
        Assert.Equal(87, checklist.SaludBateria);
        Assert.Equal("Vidrio trasero astillado", checklist.Notas);
        Assert.False(resultado.Valor.NoEnciende);
    }

    [Fact]
    public async Task Si_no_enciende_no_se_puede_poner_fecha_estimada()
    {
        var noEnciende = Checklist(("enciende", false));

        var conFecha = await Recibir().HandleAsync(Comando(noEnciende, new DateOnly(2026, 11, 10)), default);
        var sinFecha = await Recibir().HandleAsync(Comando(noEnciende), default);

        Assert.Equal("fecha_no_determinable", conFecha.Error!.Codigo);
        Assert.Contains(ItemsDeChecklist.RazonSinFecha, conFecha.Error.Mensaje);
        Assert.True(sinFecha.Valor!.NoEnciende);
        Assert.Null(sinFecha.Valor.FechaEstimadaEntrega);
    }

    [Theory]
    [InlineData("checklist_invalido", "teletransporte", 50)]
    [InlineData("bateria_invalida", "enciende", 101)]
    public async Task Valida_el_checklist(string codigo, string item, int bateria)
    {
        var checklist = new ChecklistIngreso([new ItemChecklist(item, true)], bateria, null);

        var resultado = await Recibir().HandleAsync(Comando(checklist), default);

        Assert.Equal(codigo, resultado.Error!.Codigo);
        Assert.Empty(_ordenes.Ordenes);
    }

    [Fact]
    public void Reconoce_el_formato_por_el_contenido_y_no_por_el_nombre()
    {
        Assert.Equal(("image/jpeg", "jpg"), FormatoDeImagen.Detectar(Jpeg));
        Assert.Equal(("image/png", "png"), FormatoDeImagen.Detectar(Png));
        Assert.Equal(("image/webp", "webp"), FormatoDeImagen.Detectar(Webp));
        Assert.Null(FormatoDeImagen.Detectar("%PDF-1.7 nada"u8));
        Assert.Null(FormatoDeImagen.Detectar("<svg onload=x>"u8));
    }

    [Fact]
    public async Task Guarda_la_foto_con_un_nombre_generado_y_deja_registro()
    {
        var orden = (await Recibir().HandleAsync(Comando(null), default)).Valor!;

        var resultado = await AgregarFoto().HandleAsync(orden.Id, new MemoryStream(Jpeg), Jpeg.Length, default);

        Assert.True(resultado.EsExitoso);
        var foto = Assert.Single(_ordenes.Fotos);
        Assert.Equal($"reparaciones/{orden.Id}/{foto.Id}.jpg", foto.Ruta);
        Assert.Equal(Jpeg, _almacen.Archivos[foto.Ruta]);
        Assert.Equal("foto_agregada", _auditoria.Registros[^1].Accion);

        var leida = await new ObtenerFotoHandler(_ordenes, _almacen).HandleAsync(orden.Id, foto.Id, default);
        Assert.Equal("image/jpeg", leida.Valor!.TipoContenido);
    }

    [Fact]
    public async Task Rechaza_lo_que_no_es_imagen_lo_vacio_y_mas_de_diez_fotos()
    {
        var orden = (await Recibir().HandleAsync(Comando(null), default)).Valor!;
        var pdf = "%PDF-1.7 contenido"u8.ToArray();

        var noImagen = await AgregarFoto().HandleAsync(orden.Id, new MemoryStream(pdf), pdf.Length, default);
        var vacia = await AgregarFoto().HandleAsync(orden.Id, new MemoryStream(), 0, default);
        for (var i = 0; i < FotoOrden.MaximoPorOrden; i++)
        {
            await AgregarFoto().HandleAsync(orden.Id, new MemoryStream(Png), Png.Length, default);
        }

        var unaMas = await AgregarFoto().HandleAsync(orden.Id, new MemoryStream(Png), Png.Length, default);

        Assert.Equal("formato_no_admitido", noImagen.Error!.Codigo);
        Assert.Equal("foto_demasiado_grande", vacia.Error!.Codigo);
        Assert.Equal("demasiadas_fotos", unaMas.Error!.Codigo);
        Assert.Equal(FotoOrden.MaximoPorOrden, _ordenes.Fotos.Count);
    }
}
