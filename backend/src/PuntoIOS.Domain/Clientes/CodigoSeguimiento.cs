using System.Security.Cryptography;
using System.Text;

namespace PuntoIOS.Domain.Clientes;

/// <summary>
/// Código de seguimiento del cliente para el portal público. 26 caracteres aleatorios (130 bits, CSPRNG)
/// en Base32 de Crockford, más 2 caracteres de control para detectar errores de tipeo. No se deriva de
/// ningún dato del cliente. En la base se guarda su huella (para buscarlo) y el código cifrado.
/// </summary>
public static class CodigoSeguimiento
{
    /// <summary>Base32 de Crockford: sin I, L, O ni U para evitar confusiones al leerlo.</summary>
    private const string Alfabeto = "0123456789ABCDEFGHJKMNPQRSTVWXYZ";
    private const int CaracteresAleatorios = 26;
    private const int CaracteresDeControl = 2;

    /// <summary>Primo mayor que el alfabeto: así ningún cambio de un carácter da el mismo control.</summary>
    private const int ModuloControl = 37;

    public const int Largo = CaracteresAleatorios + CaracteresDeControl;

    public static string Generar()
    {
        var caracteres = new char[Largo];
        for (var i = 0; i < CaracteresAleatorios; i++)
        {
            caracteres[i] = Alfabeto[RandomNumberGenerator.GetInt32(Alfabeto.Length)];
        }

        Control(caracteres.AsSpan(0, CaracteresAleatorios)).CopyTo(caracteres.AsSpan(CaracteresAleatorios));
        return new string(caracteres);
    }

    /// <summary>
    /// Mayúsculas, sin guiones ni espacios, y corrige las letras que se confunden (I/L → 1, O → 0).
    /// Devuelve null si queda algún carácter que no pertenece al alfabeto.
    /// </summary>
    public static string? Normalizar(string? texto)
    {
        var sb = new StringBuilder();
        foreach (var c in (texto ?? string.Empty).ToUpperInvariant())
        {
            if (c is '-' or ' ')
            {
                continue;
            }

            var corregido = c switch
            {
                'I' or 'L' => '1',
                'O' => '0',
                _ => c,
            };
            if (!Alfabeto.Contains(corregido))
            {
                return null;
            }

            sb.Append(corregido);
        }

        return sb.ToString();
    }

    /// <summary>Formato y carácter de control correctos (no dice si existe).</summary>
    public static bool EsValido(string? normalizado) =>
        normalizado is { Length: Largo }
        && normalizado.All(Alfabeto.Contains)
        && normalizado.AsSpan(CaracteresAleatorios).SequenceEqual(Control(normalizado.AsSpan(0, CaracteresAleatorios)));

    /// <summary>Huella SHA-256 (hex) del código normalizado. Es lo que se indexa para buscarlo.</summary>
    public static string Huella(string normalizado) =>
        Convert.ToHexStringLower(SHA256.HashData(Encoding.ASCII.GetBytes(normalizado)));

    /// <summary>Para mostrarlo: grupos de 4 separados por guiones.</summary>
    public static string Formatear(string normalizado) =>
        string.Join('-', normalizado.Chunk(4).Select(g => new string(g)));

    /// <summary>
    /// Suma ponderada por posición, módulo 37 (primo), escrita en 2 caracteres. Detecta siempre un carácter
    /// cambiado y dos caracteres vecinos invertidos.
    /// </summary>
    private static char[] Control(ReadOnlySpan<char> caracteres)
    {
        var suma = 0;
        for (var i = 0; i < caracteres.Length; i++)
        {
            suma += (i + 1) * Alfabeto.IndexOf(caracteres[i]);
        }

        var valor = suma % ModuloControl;
        return [Alfabeto[valor / Alfabeto.Length], Alfabeto[valor % Alfabeto.Length]];
    }
}
