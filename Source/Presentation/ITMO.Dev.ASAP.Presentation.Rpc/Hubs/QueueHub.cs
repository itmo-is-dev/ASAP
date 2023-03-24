using ITMO.Dev.ASAP.Presentation.Rpc.Abstractions;
using Microsoft.AspNetCore.SignalR;

namespace ITMO.Dev.ASAP.Presentation.Rpc.Hubs;

public class QueueHub : Hub<IQueueHubClient>
{
    public QueueHub()
    {
    }
}