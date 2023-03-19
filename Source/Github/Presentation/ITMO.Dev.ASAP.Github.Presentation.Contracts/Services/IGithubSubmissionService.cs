using ITMO.Dev.ASAP.Github.Application.Dto.Submissions;

namespace ITMO.Dev.ASAP.Github.Presentation.Contracts.Services;

public interface IGithubSubmissionService
{
    Task<GithubSubmissionDto> CreateAsync(Guid id, string organization, string repository, long pullRequestNumber);
}