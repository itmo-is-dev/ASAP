using Microsoft.Extensions.DependencyInjection;

namespace ITMO.Dev.ASAP.Controllers.Extensions;

public static class MvcBuilderExtensions
{
    public static IMvcBuilder AddCoreControllersApplicationPart(this IMvcBuilder builder)
    {
        return builder.AddApplicationPart(typeof(ICoreControllerMarker).Assembly);
    }
}