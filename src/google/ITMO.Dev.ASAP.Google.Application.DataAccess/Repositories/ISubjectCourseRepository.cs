using ITMO.Dev.ASAP.Google.Domain.SubjectCourses;

namespace ITMO.Dev.ASAP.Google.Application.DataAccess.Repositories;

public interface ISubjectCourseRepository
{
    Task<GoogleSubjectCourse?> FindByIdAsync(Guid id, CancellationToken cancellationToken);

    Task SaveAsync(GoogleSubjectCourse course, CancellationToken cancellationToken);
}