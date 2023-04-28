using Microsoft.Extensions.DependencyInjection;

namespace ITMO.Dev.ASAP.Google.Application.Handlers.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGoogleApplicationHandlers(this IServiceCollection collection)
    {
        collection.AddMediatR(x => x.RegisterServicesFromAssemblyContaining<IAssemblyMarker>());
        return collection;
    }
}