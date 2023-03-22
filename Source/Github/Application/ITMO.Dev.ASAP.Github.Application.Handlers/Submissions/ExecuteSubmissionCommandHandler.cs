using ITMO.Dev.ASAP.Common.Exceptions;
using ITMO.Dev.ASAP.Common.Resources;
using ITMO.Dev.ASAP.Github.Application.CommandExecution;
using ITMO.Dev.ASAP.Github.Application.DataAccess;
using ITMO.Dev.ASAP.Github.Application.Octokit.Notifications;
using ITMO.Dev.ASAP.Presentation.Contracts.Services;
using MediatR;
using Microsoft.Extensions.Logging;
using static ITMO.Dev.ASAP.Github.Application.Contracts.Submissions.Commands.ExecuteSubmissionCommand;

namespace ITMO.Dev.ASAP.Github.Application.Handlers.Submissions;

internal class ExecuteSubmissionCommandHandler : IRequestHandler<Command>
{
    private readonly IAsapSubmissionService _asapSubmissionService;
    private readonly IDatabaseContext _context;
    private readonly ILogger<ExecuteSubmissionCommandHandler> _logger;
    private readonly IPullRequestCommentEventNotifier _notifier;

    public ExecuteSubmissionCommandHandler(
        IAsapSubmissionService asapSubmissionService,
        IDatabaseContext context,
        ILogger<ExecuteSubmissionCommandHandler> logger,
        IPullRequestCommentEventNotifier notifier)
    {
        _asapSubmissionService = asapSubmissionService;
        _context = context;
        _logger = logger;
        _notifier = notifier;
    }

    public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
    {
        var visitor = new PullRequestContextCommandVisitor(
            _asapSubmissionService,
            _context,
            request.PullRequest,
            _notifier);

        try
        {
            await request.SubmissionCommand.AcceptAsync(visitor);
            await _notifier.ReactToUserComment(true);
        }
        catch (DomainException e)
        {
            string commandName = request.SubmissionCommand.GetType().Name;
            string title = UserCommandProcessingMessage.DomainExceptionWhileProcessingUserCommand(commandName);
            string message = $"{title} {e.Message}";

            _logger.LogError(e, "{Title}: {Message}", title, message);

            await _notifier.SendCommentToPullRequest(message);
            await _notifier.ReactToUserComment(false);
        }
        catch (Exception e)
        {
            string message = UserCommandProcessingMessage.InternalExceptionWhileProcessingUserCommand();

            _logger.LogError(e, "{Message}", message);

            await _notifier.SendCommentToPullRequest(message);
            await _notifier.ReactToUserComment(false);
        }

        return Unit.Value;
    }
}