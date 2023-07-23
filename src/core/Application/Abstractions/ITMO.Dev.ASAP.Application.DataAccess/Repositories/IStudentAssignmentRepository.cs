using ITMO.Dev.ASAP.Domain.Study;

namespace ITMO.Dev.ASAP.Application.DataAccess.Repositories;

public interface IStudentAssignmentRepository
{
    IAsyncEnumerable<StudentAssignment> GetBySubjectCourseIdAsync(
        Guid subjectCourseId,
        CancellationToken cancellationToken);
}