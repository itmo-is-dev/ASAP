using Microsoft.AspNetCore.Components.Routing;

namespace ITMO.Dev.ASAP.WebUI.AdminPanel.Navigation.Implementations;

public class NavigationService : INavigationService
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