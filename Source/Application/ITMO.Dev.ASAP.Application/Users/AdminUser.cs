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

    public bool HasAccessToSubject(Subject subject)
    {
        return true;
    }

    public bool HasAccessToSubjectCourse(SubjectCourse subjectCourse)
    {
        return true;
    }

    public bool CanUpdateAllDeadlines => true;

    public bool CanCreateUserWithRole(string roleName)
    {
        return true;
    }

    public IQueryable<Subject> FilterAvailableSubjects(IQueryable<Subject> subjects)
    {
        return subjects;
    }

    public bool CanChangeUserRole(string currentRoleName, string newRoleName)
    {
        return true;
    }
}