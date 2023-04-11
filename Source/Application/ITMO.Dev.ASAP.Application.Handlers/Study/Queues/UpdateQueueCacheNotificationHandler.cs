using ITMO.Dev.ASAP.Application.Contracts.Study.Queues.Notifications;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace ITMO.Dev.ASAP.Application.Handlers.Study.Queues;

internal class UpdateQueueCacheNotificationHandler
    : INotificationHandler<QueueUpdated.Notification>
{
    private readonly IMemoryCache _cache;
    private readonly IServiceScopeFactory _serviceProvider;

    public UpdateQueueCacheNotificationHandler(IMemoryCache cache, IServiceScopeFactory serviceProvider)
    {
        _cache = cache;
        _serviceProvider = serviceProvider;
    }

    public Task Handle(QueueUpdated.Notification notification, CancellationToken cancellationToken)
    {
        using IServiceScope serviceScope = _serviceProvider.CreateScope();
        IPublisher publisher = serviceScope.ServiceProvider.GetRequiredService<IPublisher>();

        string cacheKey = string.Concat(notification.SubjectCourseId, notification.GroupId);
        _cache.Remove(cacheKey);
        _cache.Set(cacheKey, notification.SubmissionsQueue);

        var queueUpdatedNotification = new QueueUpdated.Notification(
            notification.SubjectCourseId,
            notification.GroupId,
            notification.SubmissionsQueue);

        publisher.Publish(queueUpdatedNotification, cancellationToken);

        return Task.CompletedTask;
    }
}