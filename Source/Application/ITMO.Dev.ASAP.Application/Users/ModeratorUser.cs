using ITMO.Dev.ASAP.Application.Abstractions.Identity;
using ITMO.Dev.ASAP.Core.Study;

namespace ITMO.Dev.ASAP.Application.Users;

internal class ModeratorUser : ICurrentUser
{
    public ModeratorUser(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; }

    public IQueryable<SubjectCourse> FilterAvailableSubjectCourses(Subject subject)
    {
        return subject.Courses.AsQueryable();
    }

    public bool HasAccessToSubject(Subject subject)
    {
        return true;
    }

    public bool HasAccessToSubjectCourse(SubjectCourse subjectCourse)
    {
        return true;
    }
}