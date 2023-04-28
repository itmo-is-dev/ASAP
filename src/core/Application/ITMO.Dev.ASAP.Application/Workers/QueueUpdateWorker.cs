using ITMO.Dev.ASAP.Application.Contracts.Study.SubjectCourseGroups.Notifications;
using ITMO.Dev.ASAP.Application.Queue;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace ITMO.Dev.ASAP.Application.Workers;

public class QueueUpdateWorker : BackgroundService
{
    private static readonly TimeSpan DelayBetweenSheetUpdates = TimeSpan.FromSeconds(10);

    private readonly ILogger<QueueUpdateWorker> _logger;
    private readonly IServiceScopeFactory _serviceProvider;
    private readonly Stopwatch _stopwatch;

    private readonly QueueUpdater _updater;

    public QueueUpdateWorker(
        ILogger<QueueUpdateWorker> logger,
        IServiceScopeFactory serviceProvider,
        QueueUpdater updater)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _updater = updater;

        _stopwatch = new Stopwatch();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(DelayBetweenSheetUpdates);

        while (stoppingToken.IsCancellationRequested is false && await timer.WaitForNextTickAsync(stoppingToken))
        {
            _stopwatch.Restart();

            using IServiceScope serviceScope = _serviceProvider.CreateScope();
            IPublisher publisher = serviceScope.ServiceProvider.GetRequiredService<IPublisher>();

            IReadOnlyCollection<(Guid, Guid)> queues = _updater.Values;

            if (queues.Count is 0)
                continue;

            _logger.LogInformation("Going to update {Count} group queues", queues.Count);

            foreach ((Guid courseId, Guid groupId) in queues)
            {
                try
                {
                    var notification = new SubjectCourseGroupQueueOutdated.Notification(courseId, groupId);
                    await publisher.Publish(notification, stoppingToken);
                }
                catch (Exception e)
                {
                    _logger.LogError(
                        e,
                        "Error while updating queue for SubjectCourseId = {SubjectCourseId}, StudyGroupId = {StudyGroupId}",
                        courseId,
                        groupId);
                }
            }

            _stopwatch.Stop();

            _logger.LogInformation("Update tasks finished within {Time} ms", _stopwatch.Elapsed.TotalMilliseconds);
        }
    }
}