using Microsoft.AspNetCore.Components.Routing;

namespace ITMO.Dev.ASAP.WebUI.Abstractions.Navigation;

public interface INavigationService
{
    Task OnNavigateAsync(NavigationContext context);
}