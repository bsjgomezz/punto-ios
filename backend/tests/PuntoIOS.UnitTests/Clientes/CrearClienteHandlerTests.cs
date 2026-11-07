using PuntoIOS.Application.Common.Multilocal;
using PuntoIOS.Application.Common.Pagination;
using PuntoIOS.Application.Common.Results;
using PuntoIOS.Application.Common.Security;
using PuntoIOS.Application.Features.Clientes;
using PuntoIOS.Application.Features.Clientes.CrearCliente;
using PuntoIOS.Application.Features.Clientes.Puertos;
using PuntoIOS.Domain.Clientes;
using PuntoIOS.UnitTests.Identidad;

namespace PuntoIOS.UnitTests.Clientes;

public class CrearClienteHandlerTests
{
    private static readonly DateTimeOffset Ahora = new(2026, 11, 7, 12, 0, 0, TimeSpan.Zero);
    private static readonly Guid LocalId = Guid.NewGuid();
    private static readonly Guid UsuarioId = Guid.NewGuid();

    private sealed class ClienteRepoEnMemoria : IClienteRepositorio
    {
        public List<Cliente> Clientes { get; } = [];

        public Task<Cliente?> ObtenerPorIdAsync(Guid id, CancellationToken ct) =>
            Task.FromResult(Clientes.FirstOrDefault(c => c.Id == id));

        public Task<IReadOnlyList<Cliente>> BuscarPorTelefonoAsync(string telefonoNormalizado, CancellationToken ct) =>
            Task.FromResult<IReadOnlyList<Cliente>>(Clientes.Where(c => c.TelefonoNormalizado == telefonoNormalizado).ToList());

        public Task<PagedResult<Cliente>> BuscarAsync(string? texto, PageRequest pagina, CancellationToken ct) =>
            Task.FromResult(new PagedResult<Cliente>(Clientes, pagina.Page, pagina.PageSize, Clientes.Count));

        public void Agregar(Cliente cliente) => Clientes.Add(cliente);

        public Task GuardarCambiosAsync(CancellationToken ct) => Task.CompletedTask;
    }

    private sealed class LocalFijo : ILocalActual
    {
        public Guid LocalId => CrearClienteHandlerTests.LocalId;
    }

    private sealed class UsuarioFijo : ICurrentUserAccessor
    {
        public CurrentUser? GetCurrentUser() => new("uid", "a@b.com", true, null, UsuarioId);
    }

    private readonly ClienteRepoEnMemoria _repo = new();

    private CrearClienteHandler Crear() => new(_repo, new LocalFijo(), new UsuarioFijo(), new RelojFijo(Ahora));

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
}
