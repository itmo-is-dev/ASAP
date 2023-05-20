using ITMO.Dev.ASAP.Github.Application.DataAccess.Queries;
using ITMO.Dev.ASAP.Github.Domain.Users;

namespace ITMO.Dev.ASAP.Github.Application.DataAccess.Repositories;

public interface IGithubUserRepository
{
    IAsyncEnumerable<GithubUser> QueryAsync(GithubUserQuery query, CancellationToken cancellationToken);

    void AddRange(IReadOnlyCollection<GithubUser> users);

    void Add(GithubUser user);

    void Update(GithubUser user);
}