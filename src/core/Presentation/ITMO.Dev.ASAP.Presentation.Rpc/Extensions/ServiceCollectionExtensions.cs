using MediatR;

namespace ITMO.Dev.ASAP.Presentation.Rpc.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRpcPresentation(this IServiceCollection collection)
    {
        collection.AddSignalR();
        collection.AddMediatR(typeof(IAssemblyMarker));

        return collection;
    }
}