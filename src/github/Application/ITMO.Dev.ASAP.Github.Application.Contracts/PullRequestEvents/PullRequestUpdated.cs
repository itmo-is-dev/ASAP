using ITMO.Dev.ASAP.Github.Application.Dto.PullRequests;
using MediatR;

namespace ITMO.Dev.ASAP.Github.Application.Contracts.PullRequestEvents;

internal static class PullRequestUpdated
{
    public record Command(PullRequestDto PullRequest) : IRequest;
}