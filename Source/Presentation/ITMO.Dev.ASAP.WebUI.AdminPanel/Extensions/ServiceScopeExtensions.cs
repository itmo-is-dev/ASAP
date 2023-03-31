using ITMO.Dev.ASAP.WebUI.AdminPanel.Authorization;

namespace ITMO.Dev.ASAP.WebUI.AdminPanel.Extensions;

public static class ServiceScopeExtensions
{
    public static async ValueTask UseAdminPanelAsync(this IServiceScope scope)
    {
        IAuthorizationLoader authenticationLoader = scope.ServiceProvider.GetRequiredService<IAuthorizationLoader>();

        await authenticationLoader.LoadAsync();
    }
}