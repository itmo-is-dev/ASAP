using ITMO.Dev.ASAP.Rpc.Abstractions;
using Microsoft.AspNetCore.SignalR;

namespace ITMO.Dev.ASAP.Rpc.Hubs;

public class QueueHub : Hub<IQueueHub>
{
    public QueueHub()
    {
    }
}