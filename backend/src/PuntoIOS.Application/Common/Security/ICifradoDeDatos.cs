namespace PuntoIOS.Application.Common.Security;

/// <summary>Cifrado reversible de datos sensibles que el personal autorizado necesita volver a ver.</summary>
public interface ICifradoDeDatos
{
    string Cifrar(string texto);

    string Descifrar(string cifrado);
}
