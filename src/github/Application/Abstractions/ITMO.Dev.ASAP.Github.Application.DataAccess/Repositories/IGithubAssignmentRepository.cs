using ITMO.Dev.ASAP.Github.Application.DataAccess.Queries;
using ITMO.Dev.ASAP.Github.Domain.Assignments;

namespace ITMO.Dev.ASAP.Github.Application.DataAccess.Repositories;

public interface IGithubAssignmentRepository
{
    IAsyncEnumerable<GithubAssignment> QueryAsync(GithubAssignmentQuery query, CancellationToken cancellationToken);

    void Add(GithubAssignment assignment);

    void Update(GithubAssignment assignment);
}