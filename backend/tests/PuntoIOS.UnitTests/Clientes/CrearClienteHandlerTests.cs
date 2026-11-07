using PuntoIOS.Application.Common.Results;
using PuntoIOS.Application.Features.Clientes;
using PuntoIOS.Application.Features.Clientes.CrearCliente;
using PuntoIOS.Domain.Auditoria;
using PuntoIOS.UnitTests.Identidad;

namespace PuntoIOS.UnitTests.Clientes;

public class CrearClienteHandlerTests
{
    private static readonly DateTimeOffset Ahora = new(2026, 11, 7, 12, 0, 0, TimeSpan.Zero);
    private static readonly Guid LocalId = Guid.NewGuid();
    private static readonly Guid UsuarioId = Guid.NewGuid();

    private readonly ClienteRepoEnMemoria _repo = new();
    private readonly AuditoriaRepoEnMemoria _auditoria = new();
    private readonly DispositivoRepoEnMemoria _dispositivos = new();

    private CrearClienteHandler Crear() =>
        new(_repo, _dispositivos, _auditoria, new LocalFijo(LocalId), new UsuarioFijo(UsuarioId), new RelojFijo(Ahora));

    private static CrearClienteCommand Comando(
        string? nombre = "Ana",
        string? apellido = "Pérez",
        string? telefono = "3498 123456",
        string? email = null,
        bool permitirDuplicado = false) =>
        new(nombre, apellido, telefono, email, null, null, permitirDuplicado);

    [Fact]
    public async Task Crea_el_cliente_en_el_local_actual_y_registra_quien_lo_cargo()
    {
        var resultado = await Crear().HandleAsync(Comando(email: "Ana@Mail.com"), default);

        Assert.True(resultado.EsExitoso);
        var cliente = Assert.Single(_repo.Clientes);
        Assert.Equal(LocalId, cliente.LocalId);
        Assert.Equal(UsuarioId, cliente.CreadoPorUsuarioId);
        Assert.Equal("ana@mail.com", resultado.Valor!.Email);
    }

    [Fact]
    public async Task Deja_un_registro_de_auditoria_de_la_creacion_en_la_misma_operacion()
    {
        var resultado = await Crear().HandleAsync(Comando(), default);

        var registro = Assert.Single(_auditoria.Registros);
        Assert.Equal(AccionesAuditadas.Creado, registro.Accion);
        Assert.Equal(EntidadesAuditadas.Cliente, registro.Entidad);
        Assert.Equal(resultado.Valor!.Id, registro.EntidadId);
        Assert.Equal(UsuarioId, registro.UsuarioId);
        Assert.Equal(1, _repo.Guardados);
    }

    [Theory]
    [InlineData(null, "Pérez", "3498123456", "nombre_obligatorio")]
    [InlineData("Ana", "", "3498123456", "apellido_obligatorio")]
    [InlineData("Ana", "Pérez", " ", "telefono_obligatorio")]
    [InlineData("Ana", "Pérez", "12345", "telefono_invalido")]
    [InlineData("Ana", "Pérez", "1234567890123456", "telefono_invalido")]
    public async Task Rechaza_datos_obligatorios_faltantes_o_invalidos(
        string? nombre, string? apellido, string? telefono, string codigo)
    {
        var resultado = await Crear().HandleAsync(Comando(nombre, apellido, telefono), default);

        Assert.Equal(TipoError.Validacion, resultado.Error!.Tipo);
        Assert.Equal(codigo, resultado.Error.Codigo);
        Assert.Empty(_repo.Clientes);
        Assert.Empty(_auditoria.Registros);
    }

    [Fact]
    public async Task Rechaza_un_email_invalido_pero_acepta_que_falte()
    {
        var invalido = await Crear().HandleAsync(Comando(email: "no-es-email"), default);
        var sinEmail = await Crear().HandleAsync(Comando(email: "   "), default);

        Assert.Equal("email_invalido", invalido.Error!.Codigo);
        Assert.True(sinEmail.EsExitoso);
    }

    [Fact]
    public async Task Un_telefono_repetido_responde_conflicto_con_las_coincidencias_aunque_cambie_el_formato()
    {
        await Crear().HandleAsync(Comando(telefono: "3498123456"), default);

        var resultado = await Crear().HandleAsync(Comando(nombre: "Luis", telefono: "+54 9 3498-12-3456"), default);

        Assert.Equal(TipoError.Conflicto, resultado.Error!.Tipo);
        Assert.Equal("telefono_duplicado", resultado.Error.Codigo);
        var coincidencias = Assert.IsType<List<ClienteResumenDto>>(resultado.Error.Datos!["coincidencias"]);
        Assert.Equal("Ana", Assert.Single(coincidencias).Nombre);
        Assert.Single(_repo.Clientes);
    }

    [Fact]
    public async Task Permite_repetir_el_telefono_si_se_confirma()
    {
        await Crear().HandleAsync(Comando(telefono: "3498123456"), default);

        var resultado = await Crear().HandleAsync(Comando(nombre: "Luis", telefono: "3498123456", permitirDuplicado: true), default);

        Assert.True(resultado.EsExitoso);
        Assert.Equal(2, _repo.Clientes.Count);
    }

    private static readonly EquipoNuevo Iphone = new("Apple", "iPhone 13", "128 GB", "Azul", "490154203237518");

    [Fact]
    public async Task Con_equipo_guarda_cliente_y_equipo_juntos_con_su_auditoria()
    {
        var resultado = await Crear().HandleAsync(Comando() with { Equipo = Iphone }, default);

        Assert.True(resultado.EsExitoso);
        var equipo = Assert.Single(_dispositivos.Dispositivos);
        Assert.Equal(resultado.Valor!.Id, equipo.ClienteId);
        Assert.Equal("490154203237518", equipo.ImeiSerie);
        Assert.Equal(
            new[] { EntidadesAuditadas.Cliente, EntidadesAuditadas.Dispositivo },
            _auditoria.Registros.Select(r => r.Entidad));
        Assert.Equal(1, _repo.Guardados);
    }

    [Fact]
    public async Task Si_el_equipo_es_invalido_no_se_guarda_ni_el_cliente()
    {
        var resultado = await Crear().HandleAsync(Comando() with { Equipo = Iphone with { Modelo = " " } }, default);

        Assert.Equal("modelo_obligatorio", resultado.Error!.Codigo);
        Assert.Empty(_repo.Clientes);
        Assert.Empty(_dispositivos.Dispositivos);
    }

    [Fact]
    public async Task Si_el_imei_ya_esta_registrado_no_se_guarda_ni_el_cliente()
    {
        await Crear().HandleAsync(Comando() with { Equipo = Iphone }, default);

        var resultado = await Crear().HandleAsync(
            Comando(nombre: "Luis", telefono: "3498 999999") with { Equipo = Iphone with { ImeiSerie = "49 0154 2032 3751 8" } },
            default);

        Assert.Equal("imei_duplicado", resultado.Error!.Codigo);
        Assert.Single(_repo.Clientes);
        Assert.Single(_dispositivos.Dispositivos);
    }
}
