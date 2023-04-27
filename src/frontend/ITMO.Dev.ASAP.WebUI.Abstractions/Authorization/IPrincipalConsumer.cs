using System.Security.Claims;

namespace ITMO.Dev.ASAP.WebUI.Abstractions.Authorization;

public interface IPrincipalConsumer
{
    ValueTask OnNextAsync(ClaimsPrincipal principal);

    ValueTask OnExpiredAsync();
}