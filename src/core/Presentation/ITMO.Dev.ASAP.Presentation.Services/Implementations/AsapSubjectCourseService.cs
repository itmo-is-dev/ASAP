using ITMO.Dev.ASAP.Application.Contracts.Study.SubjectCourses.Commands;
using ITMO.Dev.ASAP.Application.Contracts.Study.SubjectCourses.Queries;
using ITMO.Dev.ASAP.Application.Dto.Study;
using ITMO.Dev.ASAP.Application.Dto.SubjectCourses;
using ITMO.Dev.ASAP.Presentation.Contracts.Services;
using MediatR;

namespace ITMO.Dev.ASAP.Presentation.Services.Implementations;

public class AsapSubjectCourseService : IAsapSubjectCourseService
{
    private readonly IMediator _mediator;

    public AsapSubjectCourseService(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<SubjectCourseDto> CreateSubjectCourseAsync(
        Guid subjectId,
        string name,
        SubmissionStateWorkflowTypeDto workflowType,
        CancellationToken cancellationToken)
    {
        var command = new CreateSubjectCourse.Command(subjectId, name, workflowType);
        CreateSubjectCourse.Response response = await _mediator.Send(command, cancellationToken);

        return response.SubjectCourse;
    }

    public async Task UpdateMentorsAsync(
        Guid subjectCourseId,
        IReadOnlyCollection<Guid> userIds,
        CancellationToken cancellationToken)
    {
        var command = new UpdateMentors.Command(subjectCourseId, userIds);
        await _mediator.Send(command, cancellationToken);
    }

    public async Task<IReadOnlyCollection<Guid>> GetSubjectCourseStudentIds(
        Guid subjectCourseId,
        CancellationToken cancellationToken)
    {
        var query = new GetSubjectCourseStudents.Query(subjectCourseId);
        GetSubjectCourseStudents.Response response = await _mediator.Send(query, cancellationToken);

        return response.StudentIds;
    }
}