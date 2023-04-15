using ITMO.Dev.ASAP.WebUI.Abstractions.Authorization;

namespace ITMO.Dev.ASAP.WebUI.Extensions;

public static class ServiceScopeExtensions
{
    public static async ValueTask UseAdminPanelAsync(this IServiceScope scope)
    {
        IAuthorizationLoader authenticationLoader = scope.ServiceProvider.GetRequiredService<IAuthorizationLoader>();

        await authenticationLoader.LoadAsync();
    }
}