using PuntoIOS.Domain.Clientes;

namespace PuntoIOS.Application.Features.Dispositivos.Puertos;

/// <summary>Puerto de persistencia de equipos de clientes. Siempre opera sobre el local actual.</summary>
public interface IDispositivoRepositorio
{
    Task<Dispositivo?> ObtenerPorIdAsync(Guid id, CancellationToken ct);

    /// <summary>Equipos de un cliente, del más reciente al más antiguo.</summary>
    Task<IReadOnlyList<Dispositivo>> ListarPorClienteAsync(Guid clienteId, CancellationToken ct);

    /// <summary>Equipo con ese IMEI o número de serie normalizado, si existe.</summary>
    Task<Dispositivo?> ObtenerPorImeiAsync(string imeiNormalizado, CancellationToken ct);

    void Agregar(Dispositivo dispositivo);

    Task GuardarCambiosAsync(CancellationToken ct);
}
