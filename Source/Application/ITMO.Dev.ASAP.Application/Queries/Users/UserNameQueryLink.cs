using ITMO.Dev.ASAP.Application.Abstractions.Tools;
using ITMO.Dev.ASAP.Application.Dto.Querying;
using ITMO.Dev.ASAP.Application.Extensions;
using ITMO.Dev.ASAP.Core.Users;

namespace ITMO.Dev.ASAP.Application.Queries.Users;

public class UserNameQueryLink : QueryLinkBase<User, UserQueryParameter>
{
    private readonly IPatternMatcher<User> _matcher;

    public UserNameQueryLink(IPatternMatcher<User> matcher)
    {
        _matcher = matcher;
    }

    protected override IQueryable<User>? TryApply(
        IQueryable<User> query,
        QueryParameter<UserQueryParameter> parameter)
    {
        if (parameter.Type is not UserQueryParameter.Name)
            return null;

        return query.Where(
            _matcher.Match(x => x.FirstName, parameter.Pattern)
                .Or(_matcher.Match(x => x.LastName, parameter.Pattern))
                .Or(_matcher.Match(x => x.MiddleName, parameter.Pattern)));
    }
}