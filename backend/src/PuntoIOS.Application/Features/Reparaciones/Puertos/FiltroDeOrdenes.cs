namespace PuntoIOS.Application.Features.Reparaciones.Puertos;

/// <summary>
/// Filtros del listado de órdenes. Todos son opcionales y se combinan (AND).
/// <c>SoloActivas</c> excluye los estados terminales (entregado y cancelado); por defecto es <c>false</c> (todas).
/// </summary>
public sealed record FiltroDeOrdenes(Guid? ClienteId = null, Guid? TecnicoId = null, bool SoloActivas = false);
