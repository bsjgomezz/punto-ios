using Microsoft.AspNetCore.DataProtection;
using PuntoIOS.Application.Common.Security;

namespace PuntoIOS.Api.Seguridad;

/// <summary>
/// Adaptador de cifrado con ASP.NET Core Data Protection (códigos de seguimiento y de desbloqueo). Las claves se guardan en el perfil del usuario
/// que corre la API (en Windows, protegidas con DPAPI). En producción hay que persistirlas en un lugar
/// compartido y respaldado: si se pierden, los códigos guardados no se pueden volver a mostrar y hay que regenerarlos.
/// </summary>
public sealed class CifradoConDataProtection(IDataProtectionProvider proveedor) : ICifradoDeDatos
{
    // El nombre del propósito se mantiene para poder descifrar los códigos ya guardados.
    private readonly IDataProtector _protector = proveedor.CreateProtector("PuntoIOS.CodigoSeguimiento.v1");

    public string Cifrar(string texto) => _protector.Protect(texto);

    public string Descifrar(string cifrado) => _protector.Unprotect(cifrado);
}
