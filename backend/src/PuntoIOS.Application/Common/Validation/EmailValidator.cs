using System.Net.Mail;

namespace PuntoIOS.Application.Common.Validation;

public static class EmailValidator
{
    /// <summary>Valida un email ya normalizado (sin espacios, en minúsculas).</summary>
    public static bool EsValido(string email)
    {
        if (email.Length is 0 or > 254)
        {
            return false;
        }

        try
        {
            return new MailAddress(email).Address == email;
        }
        catch (FormatException)
        {
            return false;
        }
    }
}
