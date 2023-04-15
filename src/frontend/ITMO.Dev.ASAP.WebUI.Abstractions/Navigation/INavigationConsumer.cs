using Microsoft.AspNetCore.Components.Routing;

namespace ITMO.Dev.ASAP.WebUI.Abstractions.Navigation;

public interface INavigationConsumer
{
    ValueTask OnNavigateAsync(NavigationContext context);
}