using ITMO.Dev.ASAP.Core.Study;

namespace ITMO.Dev.ASAP.Application.Abstractions.Identity;

public interface ICurrentUser
{
    Guid Id { get; }

    IQueryable<SubjectCourse> FilterAvailableSubjectCourses(IQueryable<SubjectCourse> subjectCourses);

    IQueryable<Subject> FilterAvailableSubjects(IQueryable<Subject> subjects);
}