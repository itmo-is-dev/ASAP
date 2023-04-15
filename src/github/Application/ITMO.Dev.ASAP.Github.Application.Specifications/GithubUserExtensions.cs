using ITMO.Dev.ASAP.Github.Common.Extensions;
using ITMO.Dev.ASAP.Github.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace ITMO.Dev.ASAP.Github.Application.Specifications;

public static class GithubUserExtensions
{
    public static IQueryable<GithubUser> ForUsername(this IQueryable<GithubUser> queryable, string username)
        => queryable.Where(x => x.Username.ToLower().Equals(username.ToLower()));

    public static async Task<GithubUser> GetForUsernameAsync(
        this IQueryable<GithubUser> queryable,
        string username,
        CancellationToken cancellationToken)
    {
        return await queryable
            .ForUsername(username)
            .SingleOrDefaultAsync(cancellationToken)
            .ThrowTaggedEntityNotFoundIfNull(username);
    }

    public static IQueryable<GithubUser> ForUsernames(
        this IQueryable<GithubUser> queryable,
        IEnumerable<string> usernames)
    {
        usernames = usernames.Select(x => x.ToLower());
        return queryable.Where(x => usernames.Contains(x.Username.ToLower()));
    }
}