using ITMO.Dev.ASAP.Application.DataAccess.Queries;
using ITMO.Dev.ASAP.Domain.Study.SubjectCourses;
using ITMO.Dev.ASAP.Domain.Study.SubjectCourses.Events;

namespace ITMO.Dev.ASAP.Application.DataAccess.Repositories;

public interface ISubjectCourseRepository
{
    IAsyncEnumerable<SubjectCourse> QueryAsync(SubjectCourseQuery query, CancellationToken cancellationToken);

    void Add(SubjectCourse subjectCourse);

    ValueTask ApplyAsync(ISubjectCourseEvent evt, CancellationToken cancellationToken);
}