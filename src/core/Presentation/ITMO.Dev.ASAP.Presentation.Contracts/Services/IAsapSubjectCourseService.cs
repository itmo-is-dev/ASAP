using ITMO.Dev.ASAP.Application.Dto.Study;
using ITMO.Dev.ASAP.Application.Dto.SubjectCourses;

namespace ITMO.Dev.ASAP.Presentation.Contracts.Services;

public interface IAsapSubjectCourseService
{
    Task<SubjectCourseDto> CreateSubjectCourseAsync(
        Guid subjectId,
        string name,
        SubmissionStateWorkflowTypeDto workflowType,
        CancellationToken cancellationToken);

    Task UpdateMentorsAsync(
        Guid subjectCourseId,
        IReadOnlyCollection<Guid> userIds,
        CancellationToken cancellationToken);

    Task<IReadOnlyCollection<Guid>> GetSubjectCourseStudentIds(
        Guid subjectCourseId,
        CancellationToken cancellationToken);
}