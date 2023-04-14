using ITMO.Dev.ASAP.Common.Logging;
using ITMO.Dev.ASAP.Github.Application.Dto.PullRequests;
using Microsoft.Extensions.Logging;

namespace ITMO.Dev.ASAP.Github.Application.Octokit.Extensions;

public static class LoggerExtensions
{
    public static ILogger ToPullRequestLogger(this ILogger logger, PullRequestDto descriptor)
    {
        string prefix = $"{descriptor.Organization}/{descriptor.Repository}/{descriptor.PullRequestNumber}";

        return new PrefixLoggerProxy(logger, prefix);
    }
}