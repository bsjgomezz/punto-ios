using PuntoIOS.Domain.Clientes;

namespace PuntoIOS.Application.Features.Clientes;

public sealed record ClienteDto(
    Guid Id,
    string Nombre,
    string Apellido,
    string Telefono,
    string? Email,
    string? Direccion,
    string? Observaciones,
    DateTimeOffset CreadoEn,
    DateTimeOffset? CodigoSeguimientoGeneradoEn = null)
{
    /// <remarks>El código de seguimiento nunca viaja en este DTO: se pide aparte y queda registrado.</remarks>
    public static ClienteDto Desde(Cliente c) =>
        new(c.Id, c.Nombre, c.Apellido, c.Telefono, c.Email, c.Direccion, c.Observaciones, c.CreadoEn, c.CodigoSeguimientoGeneradoEn);
}

/// <summary>Datos mínimos para reconocer a un cliente existente (p. ej. al avisar de un teléfono repetido).</summary>
public sealed record ClienteResumenDto(Guid Id, string Nombre, string Apellido, string Telefono)
{
    public static ClienteResumenDto Desde(Cliente c) => new(c.Id, c.Nombre, c.Apellido, c.Telefono);
}
