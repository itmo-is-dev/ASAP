using ITMO.Dev.ASAP.Github.Application.DataAccess.Queries;
using ITMO.Dev.ASAP.Github.Application.DataAccess.Repositories;
using ITMO.Dev.ASAP.Github.Common.Exceptions.Entities;
using ITMO.Dev.ASAP.Github.Common.Extensions;
using ITMO.Dev.ASAP.Github.Domain.Users;

namespace ITMO.Dev.ASAP.Github.Application.Specifications;

public static class GithubUserExtensions
{
    public static IQueryable<GithubUser> ForUsername(this IQueryable<GithubUser> queryable, string username)
        => queryable.Where(x => x.Username.ToLower().Equals(username.ToLower()));

    public static async Task<GithubUser> GetForUsernameAsync(
        this IGithubUserRepository repository,
        string username,
        CancellationToken cancellationToken = default)
    {
        var query = GithubUserQuery.Build(x => x.WithUsername(username).WithLimit(1));

        GithubUser? user = await repository
            .QueryAsync(query, cancellationToken)
            .SingleOrDefaultAsync(cancellationToken);

        return user ?? throw EntityNotFoundException.User().TaggedWithNotFound();
    }

    public static IQueryable<GithubUser> ForUsernames(
        this IQueryable<GithubUser> queryable,
        IEnumerable<string> usernames)
    {
        usernames = usernames.Select(x => x.ToLower());
        return queryable.Where(x => usernames.Contains(x.Username.ToLower()));
    }
}