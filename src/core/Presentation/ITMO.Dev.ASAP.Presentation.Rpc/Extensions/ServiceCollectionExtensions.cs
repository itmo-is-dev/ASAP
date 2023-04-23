namespace ITMO.Dev.ASAP.Presentation.Rpc.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRpcPresentation(this IServiceCollection collection)
    {
        collection.AddSignalR();
        collection.AddMediatR(x => x.RegisterServicesFromAssemblyContaining(typeof(IAssemblyMarker)));

        return collection;
    }
}