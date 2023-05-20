using ITMO.Dev.ASAP.Github.Application.DataAccess.Queries;
using ITMO.Dev.ASAP.Github.Domain.Submissions;

namespace ITMO.Dev.ASAP.Github.Application.DataAccess.Repositories;

public interface IGithubSubmissionRepository
{
    IAsyncEnumerable<GithubSubmission> QueryAsync(GithubSubmissionQuery query, CancellationToken cancellationToken);

    void Add(GithubSubmission submission);
}