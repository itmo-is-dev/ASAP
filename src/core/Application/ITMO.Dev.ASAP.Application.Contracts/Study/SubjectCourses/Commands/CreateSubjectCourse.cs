using ITMO.Dev.ASAP.Application.Dto.Study;
using ITMO.Dev.ASAP.Application.Dto.SubjectCourses;
using MediatR;

namespace ITMO.Dev.ASAP.Application.Contracts.Study.SubjectCourses.Commands;

internal static class CreateSubjectCourse
{
    public record Command(
        Guid SubjectId,
        string Title,
        SubmissionStateWorkflowTypeDto WorkflowType) : IRequest<Response>;

    public record Response(SubjectCourseDto SubjectCourse);
}