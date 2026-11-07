namespace PuntoIOS.Application.Common.Multilocal;

/// <summary>Puerto: local sobre el que opera la solicitud actual. En la etapa 1 siempre es el único local.</summary>
public interface ILocalActual
{
    Guid LocalId { get; }
}
