using Microsoft.Extensions.DependencyInjection;

namespace ITMO.Dev.ASAP.Github.Application.Handlers.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGithubApplicationHandlers(this IServiceCollection collection)
    {
        collection.AddMediatR(x => x.RegisterServicesFromAssembly(typeof(IAssemblyMarker).Assembly));
        return collection;
    }
}