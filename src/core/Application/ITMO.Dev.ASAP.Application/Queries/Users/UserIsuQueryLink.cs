using ITMO.Dev.ASAP.Application.Abstractions.Tools;
using ITMO.Dev.ASAP.Application.Dto.Querying;
using ITMO.Dev.ASAP.Application.Queries.BaseLinks;
using ITMO.Dev.ASAP.Core.UserAssociations;
using ITMO.Dev.ASAP.Core.Users;

namespace ITMO.Dev.ASAP.Application.Queries.Users;

public class UserIsuQueryLink : QueryLinkBase<User, UserQueryParameter>
{
    private readonly IPatternMatcher<User> _matcher;

    public UserIsuQueryLink(IPatternMatcher<User> matcher)
    {
        _matcher = matcher;
    }

    protected override IQueryable<User>? TryApply(
        IQueryable<User> query,
        QueryParameter<UserQueryParameter> parameter)
    {
        if (parameter.Type is not UserQueryParameter.Isu)
            return null;

        // Possible NRE if there is no Isu User Association.
#pragma warning disable CS8602
        return query.Where(
            _matcher.Match(
                x => x.Associations.OfType<IsuUserAssociation>().FirstOrDefault().UniversityId.ToString(),
                parameter.Pattern));
#pragma warning restore CS8602
    }
}