using Microsoft.AspNetCore.Components.Routing;

namespace ITMO.Dev.ASAP.WebUI.AdminPanel.Navigation;

public interface INavigationService
{
    Task OnNavigateAsync(NavigationContext context);
}