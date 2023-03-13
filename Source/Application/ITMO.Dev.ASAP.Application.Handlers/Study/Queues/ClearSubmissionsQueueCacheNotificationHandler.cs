using MediatR;
using Microsoft.Extensions.Caching.Memory;
using static ITMO.Dev.ASAP.Application.Contracts.Study.Queues.Notifications.ClearSubmissionsQueueCache;

namespace ITMO.Dev.ASAP.Application.Handlers.Study.Queues;

internal class ClearSubmissionsQueueCacheNotificationHandler : INotificationHandler<Notification>
{
    private readonly IMemoryCache _cache;

    public ClearSubmissionsQueueCacheNotificationHandler(IMemoryCache cache)
    {
        _cache = cache;
    }

    public Task Handle(Notification notification, CancellationToken cancellationToken)
    {
        string cacheKey = string.Concat(notification.SubjectCourseId, notification.GroupId);
        _cache.Remove(cacheKey);

        return Task.CompletedTask;
    }
}