using ITMO.Dev.ASAP.Application.Abstractions.Tools;
using ITMO.Dev.ASAP.Application.Dto.Querying;
using ITMO.Dev.ASAP.Core.UserAssociations;
using ITMO.Dev.ASAP.Core.Users;

namespace ITMO.Dev.ASAP.Application.Queries.Users;

public class UserGithubQueryLink : QueryLinkBase<User, UserQueryParameter>
{
    private readonly IPatternMatcher<User> _matcher;

    public UserGithubQueryLink(IPatternMatcher<User> matcher)
    {
        _matcher = matcher;
    }

    protected override IQueryable<User>? TryApply(
        IQueryable<User> query,
        QueryParameter<UserQueryParameter> parameter)
    {
        if (parameter.Type is not UserQueryParameter.GithubUsername)
            return null;

        // Possible NRE if there is no GH User Association.
#pragma warning disable CS8602
        return query.Where(_matcher.Match(
            x => x.Associations.OfType<GithubUserAssociation>().FirstOrDefault().GithubUsername,
            parameter.Pattern));
#pragma warning restore CS8602
    }
}