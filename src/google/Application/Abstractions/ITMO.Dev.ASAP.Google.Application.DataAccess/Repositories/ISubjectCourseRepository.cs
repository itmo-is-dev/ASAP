using ITMO.Dev.ASAP.Google.Application.DataAccess.Queries;
using ITMO.Dev.ASAP.Google.Domain.SubjectCourses;

namespace ITMO.Dev.ASAP.Google.Application.DataAccess.Repositories;

public interface ISubjectCourseRepository
{
    Task<GoogleSubjectCourse?> FindByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<IEnumerable<GoogleSubjectCourse>> QueryAsync(SubjectCourseQuery query, CancellationToken cancellationToken);

    Task AddAsync(GoogleSubjectCourse course, CancellationToken cancellationToken);
}