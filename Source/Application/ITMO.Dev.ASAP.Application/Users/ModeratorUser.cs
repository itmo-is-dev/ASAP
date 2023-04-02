using ITMO.Dev.ASAP.Application.Abstractions.Identity;
using ITMO.Dev.ASAP.Core.Study;
using ITMO.Dev.ASAP.Identity.Abstractions.Models;

namespace ITMO.Dev.ASAP.Application.Users;

internal class ModeratorUser : ICurrentUser
{
    private static readonly HashSet<string?> AvailableRolesToChange = new()
    {
        AsapIdentityRoleNames.MentorRoleName,
    };

    public ModeratorUser(Guid id)
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

    public IQueryable<Subject> FilterAvailableSubjects(IQueryable<Subject> subjects)
    {
        return subjects;
    }

    public bool CanUpdateAllDeadlines => true;

    public bool CanCreateUserWithRole(string roleName)
    {
        return AvailableRolesToChange.Contains(roleName);
    }

    public bool CanChangeUserRole(string currentRoleName, string newRoleName)
    {
        return AvailableRolesToChange.Contains(currentRoleName)
               && AvailableRolesToChange.Contains(newRoleName);
    }
}