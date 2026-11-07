using PuntoIOS.Application.Common.Multilocal;
using PuntoIOS.Domain.Locales;

namespace PuntoIOS.Infrastructure.Persistence;

/// <summary>Etapa 1: un solo local. Cuando haya varios, este adaptador lo resolverá según el usuario o la solicitud.</summary>
public sealed class LocalUnico : ILocalActual
{
    public Guid LocalId => LocalesDeSistema.PrincipalId;
}
