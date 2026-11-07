using PuntoIOS.Domain.Clientes;

namespace PuntoIOS.UnitTests.Clientes;

public class ClienteTests
{
    private static readonly DateTimeOffset Ahora = new(2026, 11, 7, 12, 0, 0, TimeSpan.Zero);

    [Theory]
    [InlineData("3498123456", "3498123456")]
    [InlineData("+54 9 3498 12-3456", "3498123456")]
    [InlineData("(03498) 12 3456", "3498123456")]
    [InlineData("12-3456", "123456")]
    [InlineData("abc", "")]
    [InlineData(null, "")]
    public void Normalizar_telefono_deja_los_ultimos_10_digitos(string? entrada, string esperado)
    {
        Assert.Equal(esperado, Cliente.NormalizarTelefono(entrada));
    }

    [Fact]
    public void Registrar_limpia_espacios_pasa_el_email_a_minusculas_y_vacios_a_nulo()
    {
        var cliente = Cliente.Registrar(
            Guid.NewGuid(), " Ana ", " Pérez ", " 3498 123456 ", " ANA@Mail.com ", "  ", null, null, Ahora);

        Assert.Equal("Ana", cliente.Nombre);
        Assert.Equal("Pérez", cliente.Apellido);
        Assert.Equal("3498 123456", cliente.Telefono);
        Assert.Equal("3498123456", cliente.TelefonoNormalizado);
        Assert.Equal("ana@mail.com", cliente.Email);
        Assert.Null(cliente.Direccion);
        Assert.Null(cliente.Observaciones);
    }

    [Theory]
    [InlineData("", "Pérez", "3498123456")]
    [InlineData("Ana", " ", "3498123456")]
    [InlineData("Ana", "Pérez", "12345")]
    public void Registrar_rechaza_datos_obligatorios_invalidos(string nombre, string apellido, string telefono)
    {
        Assert.Throws<ArgumentException>(() =>
            Cliente.Registrar(Guid.NewGuid(), nombre, apellido, telefono, null, null, null, null, Ahora));
    }

    [Fact]
    public void Actualizar_devuelve_solo_lo_que_cambio_y_actualiza_el_telefono_normalizado()
    {
        var cliente = Cliente.Registrar(Guid.NewGuid(), "Ana", "Pérez", "3498 111111", null, null, null, null, Ahora);

        var cambios = cliente.Actualizar("Ana", "Pérez", "+54 9 3498 22-2222", "NUEVO@mail.com", null, "  ");

        Assert.Equal(new[] { "telefono", "email" }, cambios.Select(c => c.Campo));
        Assert.Equal("3498222222", cliente.TelefonoNormalizado);
        Assert.Equal("nuevo@mail.com", cliente.Email);
        Assert.Null(cliente.Observaciones);
    }

    [Fact]
    public void Actualizar_sin_cambios_devuelve_lista_vacia()
    {
        var cliente = Cliente.Registrar(Guid.NewGuid(), "Ana", "Pérez", "3498 111111", "a@b.com", "Calle 1", "VIP", null, Ahora);

        Assert.Empty(cliente.Actualizar(" Ana ", "Pérez", "3498 111111", "A@B.com", "Calle 1", "VIP"));
    }
}
