using PuntoIOS.Domain.Clientes;

namespace PuntoIOS.Application.Features.Dispositivos;

/// <summary>Equipo tal como lo ve cualquier usuario con acceso: el IMEI va siempre enmascarado.</summary>
public sealed record DispositivoDto(
    Guid Id,
    Guid ClienteId,
    string Marca,
    string Modelo,
    string? Almacenamiento,
    string? Color,
    string? ImeiSerieEnmascarado,
    string? Observaciones,
    DateTimeOffset CreadoEn)
{
    public static DispositivoDto Desde(Dispositivo d) =>
        new(d.Id, d.ClienteId, d.Marca, d.Modelo, d.Almacenamiento, d.Color, d.ImeiSerieEnmascarado, d.Observaciones, d.CreadoEn);
}

/// <summary>IMEI o número de serie completo. Solo se entrega con el permiso dispositivos.ver_imei.</summary>
public sealed record ImeiDto(string? ImeiSerie);
