using System.Security.Claims;

namespace ITMO.Dev.ASAP.WebUI.AdminPanel.Authorization;

public interface IPrincipalConsumer
{
    ValueTask OnNextAsync(ClaimsPrincipal principal);

    ValueTask OnExpiredAsync();
}