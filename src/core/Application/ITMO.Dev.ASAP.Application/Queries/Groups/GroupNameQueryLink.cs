using ITMO.Dev.ASAP.Application.DataAccess.Queries;
using ITMO.Dev.ASAP.Application.Dto.Querying;
using ITMO.Dev.ASAP.Application.Queries.BaseLinks;

namespace ITMO.Dev.ASAP.Application.Queries.Groups;

public class GroupNameQueryLink : QueryLinkBase<StudentGroupQuery.Builder, GroupQueryParameter>
{
    protected override StudentGroupQuery.Builder? TryApply(
        StudentGroupQuery.Builder queryBuilder,
        QueryParameter<GroupQueryParameter> parameter)
    {
        return parameter.Type is not GroupQueryParameter.Name
            ? null
            : queryBuilder.WithNamePattern(parameter.Pattern);
    }
}