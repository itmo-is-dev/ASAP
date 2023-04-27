using ITMO.Dev.ASAP.Application.Dto.Study;
using ITMO.Dev.ASAP.Github.Application.Dto.SubjectCourses;
using MediatR;

namespace ITMO.Dev.ASAP.Github.Application.Contracts.SubjectCourses.Commands;

public static class CreateSubjectCourse
{
    public record Command(
        Guid SubjectId,
        string Name,
        SubmissionStateWorkflowTypeDto WorkflowType,
        string GithubOrganizationName,
        string TemplateRepositoryName,
        string MentorTeamName) : IRequest<Response>;

    public record Response(GithubSubjectCourseDto SubjectCourse);
}