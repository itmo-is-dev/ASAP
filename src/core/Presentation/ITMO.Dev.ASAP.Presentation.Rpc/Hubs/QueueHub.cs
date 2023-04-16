using ITMO.Dev.ASAP.Presentation.Rpc.Abstractions;
using Microsoft.AspNetCore.SignalR;

namespace ITMO.Dev.ASAP.Presentation.Rpc.Hubs;

public class QueueHub : Hub<IQueueHubClient>
{
    public QueueHub()
    {
    }

    public Task QueueUpdateSubscribeAsync(Guid courseId, Guid groupId)
    {
        return Groups.AddToGroupAsync(Context.ConnectionId, CombineIdentifiers(courseId, groupId));
    }

    public Task QueueUpdateUnsubscribeAsync(Guid courseId, Guid groupId)
    {
        return Groups.RemoveFromGroupAsync(Context.ConnectionId, CombineIdentifiers(courseId, groupId));
    }

    private static string CombineIdentifiers(Guid courseId, Guid groupId)
    {
        return string.Concat(courseId, groupId);
    }
}