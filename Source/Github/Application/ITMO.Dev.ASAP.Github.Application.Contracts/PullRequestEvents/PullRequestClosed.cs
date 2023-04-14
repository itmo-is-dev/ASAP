using ITMO.Dev.ASAP.Github.Application.Dto.PullRequests;
using MediatR;

namespace ITMO.Dev.ASAP.Github.Application.Contracts.PullRequestEvents;

internal static class PullRequestClosed
{
    public record Command(PullRequestDto PullRequest, bool IsMerged) : IRequest;
}