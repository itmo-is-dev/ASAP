using ITMO.Dev.ASAP.Application.Dto.Querying;
using ITMO.Dev.ASAP.Application.Dto.Users;
using ITMO.Dev.ASAP.Application.Queries.BaseLinks;
using System.Text.RegularExpressions;

namespace ITMO.Dev.ASAP.Application.Queries.Students;

public class StudentGithubFilterLink : FilterLinkBase<StudentDto, StudentQueryParameter>
{
    protected override IEnumerable<StudentDto>? TryApply(
        IEnumerable<StudentDto> data,
        QueryParameter<StudentQueryParameter> parameter)
    {
        if (parameter.Type is not StudentQueryParameter.GithubUsername)
            return null;

        var regex = new Regex(parameter.Pattern);

        return data.Where(x => x.GitHubUsername is not null && regex.IsMatch(x.GitHubUsername));
    }
}