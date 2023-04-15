using ITMO.Dev.ASAP.Commands.SubmissionCommands;
using ITMO.Dev.ASAP.Github.Application.Dto.PullRequests;
using MediatR;

namespace ITMO.Dev.ASAP.Github.Application.Contracts.Submissions.Commands;

public static class ExecuteSubmissionCommand
{
    public record Command(PullRequestDto PullRequest, long CommentId, ISubmissionCommand SubmissionCommand) : IRequest;
}