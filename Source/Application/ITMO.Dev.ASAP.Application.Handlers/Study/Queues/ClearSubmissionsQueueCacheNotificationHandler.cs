using ITMO.Dev.ASAP.Application.Abstractions.Google.Notifications;
using MediatR;
using Microsoft.Extensions.Caching.Memory;

namespace ITMO.Dev.ASAP.Application.Handlers.Study.Queues;

internal class ClearSubmissionsQueueCacheNotificationHandler
    : INotificationHandler<SubjectCourseGroupQueueUpdatedNotification>
{
    private readonly IMemoryCache _cache;

    public ClearSubmissionsQueueCacheNotificationHandler(IMemoryCache cache)
    {
        _cache = cache;
    }

    public Task Handle(SubjectCourseGroupQueueUpdatedNotification notification, CancellationToken cancellationToken)
    {
        string cacheKey = string.Concat(notification.SubjectCourseId, notification.GroupId);
        _cache.Remove(cacheKey);

        return Task.CompletedTask;
    }
}