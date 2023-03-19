using ITMO.Dev.ASAP.Github.Application.Contracts.SubjectCourses.Commands;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ITMO.Dev.ASAP.Github.Application.BackgroundServices;

public class GithubInviteBackgroundService : BackgroundService
{
    /// <summary>
    ///     This worker is our restriction bypass, github allow to invite only 50 users per 24 hours.
    ///     So we need to send invites every 24 hours + 1 minutes shift for preventing race conditions.
    ///     But in case of restart it is better to try many time. Anyways we have logic for stop inviting after first fail.
    /// </summary>
    private readonly TimeSpan _delayBetweenInviteIteration = TimeSpan.FromHours(6);

    private readonly ILogger<GithubInviteBackgroundService> _logger;
    private readonly IServiceScopeFactory _serviceProvider;

    public GithubInviteBackgroundService(
        IServiceScopeFactory serviceProvider,
        ILogger<GithubInviteBackgroundService> logger)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(_delayBetweenInviteIteration);

        while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                using IServiceScope scope = _serviceProvider.CreateScope();

                IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                var command = new UpdateSubjectCourseOrganizations.Command();
                await mediator.Send(command, stoppingToken);
            }
            catch (Exception ex)
            {
                const string template = "Failed to execute GithubInvitingWorker with exception message {Message}.";
                _logger.LogError(ex, template, ex.Message);
            }
        }
    }
}