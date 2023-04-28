using ITMO.Dev.ASAP.Application.Contracts.Study.SubjectCourses.Notifications;
using ITMO.Dev.ASAP.Application.SubjectCourses;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace ITMO.Dev.ASAP.Application.Workers;

public class SubjectCoursePointsUpdateWorker : BackgroundService
{
    private static readonly TimeSpan DelayBetweenSheetUpdates = TimeSpan.FromSeconds(10);

    private readonly ILogger<SubjectCoursePointsUpdateWorker> _logger;
    private readonly IServiceScopeFactory _serviceProvider;
    private readonly Stopwatch _stopwatch;

    private readonly SubjectCourseUpdater _updater;

    public SubjectCoursePointsUpdateWorker(
        ILogger<SubjectCoursePointsUpdateWorker> logger,
        IServiceScopeFactory serviceProvider,
        SubjectCourseUpdater updater)
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

            IReadOnlyCollection<Guid> queues = _updater.PointUpdates;

            if (queues.Count is 0)
                continue;

            _logger.LogInformation("Going to update {Count} group queues", queues.Count);

            foreach (Guid subjectCourseId in queues)
            {
                try
                {
                    var notification = new SubjectCoursePointsOutdated.Notification(subjectCourseId);
                    await publisher.Publish(notification, stoppingToken);
                }
                catch (Exception e)
                {
                    _logger.LogError(
                        e,
                        "Error while updating points for SubjectCourseId = {CourseId}",
                        subjectCourseId);
                }
            }

            _stopwatch.Stop();

            _logger.LogInformation("Update tasks finished within {Time} ms", _stopwatch.Elapsed.TotalMilliseconds);
        }
    }
}