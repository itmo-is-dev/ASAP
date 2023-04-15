using ITMO.Dev.ASAP.WebUI.Abstractions.Navigation;
using Microsoft.AspNetCore.Components.Routing;

namespace ITMO.Dev.ASAP.WebUI.Application.Navigation;

internal class NavigationService : INavigationService
{
    private readonly IEnumerable<INavigationConsumer> _navigationConsumers;

    public NavigationService(IEnumerable<INavigationConsumer> navigationConsumers)
    {
        _navigationConsumers = navigationConsumers;
    }

    public async Task OnNavigateAsync(NavigationContext context)
    {
        foreach (INavigationConsumer consumer in _navigationConsumers)
        {
            await consumer.OnNavigateAsync(context);
        }
    }
}