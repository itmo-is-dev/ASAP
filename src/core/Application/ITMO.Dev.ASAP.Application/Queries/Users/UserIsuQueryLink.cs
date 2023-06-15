using ITMO.Dev.ASAP.Application.DataAccess.Queries;
using ITMO.Dev.ASAP.Application.Dto.Querying;
using ITMO.Dev.ASAP.Application.Queries.BaseLinks;

namespace ITMO.Dev.ASAP.Application.Queries.Users;

public class UserIsuQueryLink : QueryLinkBase<UserQuery.Builder, UserQueryParameter>
{
    protected override UserQuery.Builder? TryApply(
        UserQuery.Builder queryBuilder,
        QueryParameter<UserQueryParameter> parameter)
    {
        return parameter.Type is not UserQueryParameter.Isu
            ? null
            : queryBuilder.WithUniversityIdPattern(parameter.Pattern);
    }
}