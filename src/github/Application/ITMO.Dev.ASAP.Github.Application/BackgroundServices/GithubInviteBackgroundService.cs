using ITMO.Dev.ASAP.Github.Application.Contracts.SubjectCourses.Commands;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ITMO.Dev.ASAP.Github.Application.BackgroundServices;

public class GithubInviteBackgroundService : BackgroundService
{
    private readonly TimeSpan _delayBetweenInviteIteration;
    private readonly ILogger<GithubInviteBackgroundService> _logger;
    private readonly IServiceScopeFactory _serviceProvider;

    public GithubInviteBackgroundService(
        IServiceScopeFactory serviceProvider,
        GithubInviteBackgroundServiceConfiguration config,
        ILogger<GithubInviteBackgroundService> logger)
    {
        _logger = logger;
        _delayBetweenInviteIteration = config.Delay;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(_delayBetweenInviteIteration);

        do
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
        while (stoppingToken.IsCancellationRequested is false && await timer.WaitForNextTickAsync(stoppingToken));
    }
}