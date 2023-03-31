using Microsoft.AspNetCore.Components.Routing;

namespace ITMO.Dev.ASAP.WebUI.AdminPanel.Navigation;

public interface INavigationConsumer
{
    ValueTask OnNavigateAsync(NavigationContext context);
}