using ITMO.Dev.ASAP.Github.Application.Dto.SubjectCourses;
using MediatR;

namespace ITMO.Dev.ASAP.Github.Application.Contracts.SubjectCourses.Queries;

public static class GetSubjectCourseByOrganizationName
{
    public record Query(string OrganizationName) : IRequest<Response>;

    public record Response(GithubSubjectCourseDto SubjectCourse);
}