using ITMO.Dev.ASAP.Application.DataAccess.Queries;
using ITMO.Dev.ASAP.Application.Dto.Querying;
using ITMO.Dev.ASAP.Application.Queries.BaseLinks;

namespace ITMO.Dev.ASAP.Application.Queries.Students;

public class StudentNameQueryLink : QueryLinkBase<StudentQuery.Builder, StudentQueryParameter>
{
    protected override StudentQuery.Builder? TryApply(
        StudentQuery.Builder queryBuilder,
        QueryParameter<StudentQueryParameter> parameter)
    {
        return parameter.Type is not StudentQueryParameter.Name
            ? null
            : queryBuilder.WithFullNamePattern(parameter.Pattern);
    }
}