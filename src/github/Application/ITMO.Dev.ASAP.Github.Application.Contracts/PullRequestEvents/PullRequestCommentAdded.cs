using ITMO.Dev.ASAP.Github.Application.Dto.PullRequests;
using MediatR;

namespace ITMO.Dev.ASAP.Github.Application.Contracts.PullRequestEvents;

public static class PullRequestCommentAdded
{
    public record Command(PullRequestDto PullRequest, long CommentId) : IRequest;
}