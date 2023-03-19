using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace ITMO.Dev.ASAP.Github.Application.Handlers.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationHandlers(this IServiceCollection collection)
    {
        collection.AddMediatR(typeof(IAssemblyMarker));
        return collection;
    }
}