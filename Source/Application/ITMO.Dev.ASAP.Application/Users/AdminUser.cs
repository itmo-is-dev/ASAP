using ITMO.Dev.ASAP.Application.Abstractions.Identity;
using ITMO.Dev.ASAP.Core.Study;

namespace ITMO.Dev.ASAP.Application.Users;

internal class AdminUser : ICurrentUser
{
    public AdminUser(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; }

    public IQueryable<SubjectCourse> FilterAvailableSubjectCourses(IQueryable<SubjectCourse> subjectCourses)
    {
        return subjectCourses;
    }

    public IQueryable<Subject> FilterAvailableSubjects(IQueryable<Subject> subjects)
    {
        return subjects;
    }
}