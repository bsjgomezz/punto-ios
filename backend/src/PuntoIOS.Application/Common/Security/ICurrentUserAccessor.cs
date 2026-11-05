namespace PuntoIOS.Application.Common.Security;

/// <summary>Puerto: entrega el usuario de la solicitud actual, o null si no hay sesión válida.</summary>
public interface ICurrentUserAccessor
{
    CurrentUser? GetCurrentUser();
}
