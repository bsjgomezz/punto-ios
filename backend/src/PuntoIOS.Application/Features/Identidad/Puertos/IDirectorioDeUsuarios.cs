namespace PuntoIOS.Application.Features.Identidad.Puertos;

/// <summary>Contrato del módulo Identidad para que otros módulos muestren quién hizo algo, sin tocar sus tablas.</summary>
public interface IDirectorioDeUsuarios
{
    Task<IReadOnlyDictionary<Guid, string>> ObtenerNombresAsync(IEnumerable<Guid> usuarioIds, CancellationToken ct);
}
