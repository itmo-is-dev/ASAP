using ITMO.Dev.ASAP.Github.Application.Contracts.SubjectCourses.Queries;
using ITMO.Dev.ASAP.Github.Application.Dto.SubjectCourses;
using ITMO.Dev.ASAP.Github.Presentation.Contracts.Services;
using MediatR;

namespace ITMO.Dev.ASAP.Github.Presentation.Services.Implementations;

public class GithubSubjectCourseService : IGithubSubjectCourseService
{
    private readonly IMediator _mediator;

    public GithubSubjectCourseService(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<GithubSubjectCourseDto> GetByOrganizationName(
        string organizationName,
        CancellationToken cancellationToken)
    {
        var query = new GetSubjectCourseByOrganizationName.Query(organizationName);
        GetSubjectCourseByOrganizationName.Response response = await _mediator.Send(query, cancellationToken);

        return response.SubjectCourse;
    }
}