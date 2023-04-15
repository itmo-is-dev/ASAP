using ITMO.Dev.ASAP.Application.Abstractions.Google.Notifications;
using ITMO.Dev.ASAP.Application.Google.Services;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace ITMO.Dev.ASAP.Application.Google.Workers;

public class GoogleTableUpdateWorker : BackgroundService
{
    private static readonly TimeSpan DelayBetweenSheetUpdates = TimeSpan.FromSeconds(10);
    private readonly ILogger<GoogleTableUpdateWorker> _logger;
    private readonly IServiceScopeFactory _serviceProvider;
    private readonly Stopwatch _stopwatch;

    private readonly TableUpdateQueue _tableUpdateQueue;

    public GoogleTableUpdateWorker(
        ILogger<GoogleTableUpdateWorker> logger,
        IServiceScopeFactory serviceProvider,
        TableUpdateQueue tableUpdateQueue)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _stopwatch = new Stopwatch();

        _tableUpdateQueue = tableUpdateQueue;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // TODO: return to queue on fail
        // TODO: if something will went wrong loop is won't run any more until restart.
        using var timer = new PeriodicTimer(DelayBetweenSheetUpdates);

        while (stoppingToken.IsCancellationRequested is false && await timer.WaitForNextTickAsync(stoppingToken))
        {
            _stopwatch.Restart();

            bool pointsTableUpdated = await UpdateTablePoints(stoppingToken);
            bool queueTableUpdated = await UpdateTableQueue(stoppingToken);

            _stopwatch.Stop();

            if (pointsTableUpdated || queueTableUpdated)
                _logger.LogInformation("Update tasks finished within {Time} ms", _stopwatch.Elapsed.TotalMilliseconds);
        }
    }

    private async Task<bool> UpdateTablePoints(CancellationToken cancellationToken)
    {
        using IServiceScope serviceScope = _serviceProvider.CreateScope();
        IPublisher publisher = serviceScope.ServiceProvider.GetRequiredService<IPublisher>();

        IReadOnlyCollection<Guid> subjectCourses = _tableUpdateQueue.PointsUpdateSubjectCourseIds
            .GetAndClearValues();

        if (subjectCourses.Any())
            _logger.LogInformation("Going to update {Count} subject courses points", subjectCourses.Count);

        foreach (Guid subjectCourseId in subjectCourses)
        {
            var notification = new SubjectCoursePointsUpdatedNotification(subjectCourseId);
            await publisher.Publish(notification, cancellationToken);
        }

        return subjectCourses.Any();
    }

    private async Task<bool> UpdateTableQueue(CancellationToken cancellationToken)
    {
        using IServiceScope serviceScope = _serviceProvider.CreateScope();
        IPublisher publisher = serviceScope.ServiceProvider.GetRequiredService<IPublisher>();

        IReadOnlyCollection<(Guid, Guid)> queues = _tableUpdateQueue.QueueUpdateSubjectCourseGroupIds
            .GetAndClearValues();

        if (queues.Any())
            _logger.LogInformation("Going to update {Count} group queues", queues.Count);

        foreach ((Guid courseId, Guid groupId) in queues)
        {
            var notification = new SubjectCourseGroupQueueUpdateNotification(courseId, groupId);
            await publisher.Publish(notification, cancellationToken);
        }

        return queues.Any();
    }
}