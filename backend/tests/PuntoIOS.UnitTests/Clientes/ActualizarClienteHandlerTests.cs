using PuntoIOS.Application.Common.Pagination;
using PuntoIOS.Application.Common.Results;
using PuntoIOS.Application.Features.Clientes;
using PuntoIOS.Application.Features.Clientes.ActualizarCliente;
using PuntoIOS.Application.Features.Clientes.HistorialCliente;
using PuntoIOS.Domain.Auditoria;
using PuntoIOS.Domain.Clientes;
using PuntoIOS.UnitTests.Identidad;

namespace PuntoIOS.UnitTests.Clientes;

public class ActualizarClienteHandlerTests
{
    private static readonly DateTimeOffset Ahora = new(2026, 11, 7, 12, 0, 0, TimeSpan.Zero);
    private static readonly Guid LocalId = Guid.NewGuid();
    private static readonly Guid UsuarioId = Guid.NewGuid();

    private readonly ClienteRepoEnMemoria _repo = new();
    private readonly AuditoriaRepoEnMemoria _auditoria = new();

    private ActualizarClienteHandler Crear() =>
        new(_repo, _auditoria, new LocalFijo(LocalId), new UsuarioFijo(UsuarioId), new RelojFijo(Ahora));

    private Cliente Existente(string nombre = "Ana", string apellido = "Pérez", string telefono = "3498 111111")
    {
        var cliente = Cliente.Registrar(LocalId, nombre, apellido, telefono, "ana@mail.com", null, null, null, Ahora);
        _repo.Clientes.Add(cliente);
        return cliente;
    }

    private static ActualizarClienteCommand Comando(
        Guid id,
        string nombre = "Ana",
        string apellido = "Pérez",
        string telefono = "3498 111111",
        string? email = "ana@mail.com",
        string? direccion = null,
        string? observaciones = null,
        bool permitirDuplicado = false) =>
        new(id, nombre, apellido, telefono, email, direccion, observaciones, permitirDuplicado);

    [Fact]
    public async Task Registra_solo_los_campos_que_cambiaron_con_su_valor_anterior_y_nuevo()
    {
        var cliente = Existente();

        var resultado = await Crear().HandleAsync(
            Comando(cliente.Id, apellido: "Gómez", direccion: "Calle 1", email: ""), default);

        Assert.True(resultado.EsExitoso);
        var registro = Assert.Single(_auditoria.Registros);
        Assert.Equal(AccionesAuditadas.Editado, registro.Accion);
        Assert.Equal(UsuarioId, registro.UsuarioId);
        Assert.Equal(
            new[]
            {
                new CambioDeCampo("apellido", "Pérez", "Gómez"),
                new CambioDeCampo("email", "ana@mail.com", null),
                new CambioDeCampo("direccion", null, "Calle 1"),
            },
            registro.Cambios.ToArray());
        Assert.Equal("Gómez", cliente.Apellido);
    }

    [Fact]
    public async Task Si_no_cambia_nada_no_registra_ni_guarda()
    {
        var cliente = Existente();

        var resultado = await Crear().HandleAsync(Comando(cliente.Id), default);

        Assert.True(resultado.EsExitoso);
        Assert.Empty(_auditoria.Registros);
        Assert.Equal(0, _repo.Guardados);
    }

    [Fact]
    public async Task Cliente_inexistente_responde_no_encontrado()
    {
        var resultado = await Crear().HandleAsync(Comando(Guid.NewGuid()), default);

        Assert.Equal(TipoError.NoEncontrado, resultado.Error!.Tipo);
    }

    [Fact]
    public async Task Rechaza_datos_invalidos_sin_modificar_el_cliente()
    {
        var cliente = Existente();

        var resultado = await Crear().HandleAsync(Comando(cliente.Id, nombre: ""), default);

        Assert.Equal("nombre_obligatorio", resultado.Error!.Codigo);
        Assert.Equal("Ana", cliente.Nombre);
    }

    [Fact]
    public async Task Cambiar_a_un_telefono_de_otro_cliente_avisa_pero_se_puede_confirmar()
    {
        var ana = Existente();
        Existente("Luis", "Gómez", "3498 222222");

        var aviso = await Crear().HandleAsync(Comando(ana.Id, telefono: "+54 9 3498 22 2222"), default);
        Assert.Equal("telefono_duplicado", aviso.Error!.Codigo);
        Assert.Equal("3498 111111", ana.Telefono);

        var confirmado = await Crear().HandleAsync(Comando(ana.Id, telefono: "3498 222222", permitirDuplicado: true), default);
        Assert.True(confirmado.EsExitoso);
        Assert.Equal("3498 222222", ana.Telefono);
    }

    [Fact]
    public async Task Guardar_con_el_mismo_telefono_propio_no_cuenta_como_duplicado()
    {
        var ana = Existente();

        var resultado = await Crear().HandleAsync(Comando(ana.Id, nombre: "Anita", telefono: "(3498) 11-1111"), default);

        Assert.True(resultado.EsExitoso);
    }

    [Fact]
    public async Task El_historial_devuelve_lo_mas_reciente_primero_con_el_nombre_de_quien_edito()
    {
        var cliente = Existente();
        _auditoria.Registros.Add(RegistroAuditoria.Registrar(
            LocalId, EntidadesAuditadas.Cliente, cliente.Id, AccionesAuditadas.Creado, UsuarioId, Ahora.AddDays(-1)));
        await Crear().HandleAsync(Comando(cliente.Id, apellido: "Gómez"), default);
        var handler = new HistorialClienteHandler(_repo, _auditoria, new DirectorioFijo(new() { [UsuarioId] = "Recepción 1" }));

        var resultado = await handler.HandleAsync(cliente.Id, new PageRequest(), default);

        Assert.True(resultado.EsExitoso);
        var items = resultado.Valor!.Items;
        Assert.Equal(new[] { AccionesAuditadas.Editado, AccionesAuditadas.Creado }, items.Select(i => i.Accion));
        Assert.Equal("Recepción 1", items[0].Usuario);
        Assert.Equal("apellido", Assert.Single(items[0].Cambios).Campo);
    }

    [Fact]
    public async Task El_historial_de_un_cliente_inexistente_responde_no_encontrado()
    {
        var handler = new HistorialClienteHandler(_repo, _auditoria, new DirectorioFijo(new Dictionary<Guid, string>()));

        var resultado = await handler.HandleAsync(Guid.NewGuid(), new PageRequest(), default);

        Assert.Equal(TipoError.NoEncontrado, resultado.Error!.Tipo);
    }
}
