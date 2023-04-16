using ITMO.Dev.ASAP.Presentation.Rpc.Hubs;

namespace ITMO.Dev.ASAP.Presentation.Rpc.Extensions;

public static class WebApplicationExtensions
{
    public static void UseRpcPresentation(this IEndpointRouteBuilder builder)
    {
        builder.MapHub<QueueHub>("hubs/queue");
    }
}