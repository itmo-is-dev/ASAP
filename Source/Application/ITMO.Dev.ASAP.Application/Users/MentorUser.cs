using ITMO.Dev.ASAP.Application.Abstractions.Identity;
using ITMO.Dev.ASAP.Core.Study;

namespace ITMO.Dev.ASAP.Application.Users;

internal class MentorUser : ICurrentUser
{
    public MentorUser(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; }

    public bool HasAccessToSubject(Subject subject)
    {
        return subject.Courses.Any(HasAccessToSubjectCourse);
    }

    public bool HasAccessToSubjectCourse(SubjectCourse subjectCourse)
    {
        return subjectCourse.Mentors.Any(m => m.UserId == Id);
    }

    public IQueryable<Subject> FilterAvailableSubjects(IQueryable<Subject> subjects)
    {
        return subjects.Where(s => HasAccessToSubject(s)).AsQueryable();
    }

    public bool CanUpdateAllDeadlines => false;

    public bool CanCreateUserWithRole(string roleName)
    {
        return false;
    }

    public bool CanChangeUserRole(string currentRoleName, string newRoleName)
    {
        return false;
    }
}