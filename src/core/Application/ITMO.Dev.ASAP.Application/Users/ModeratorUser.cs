using ITMO.Dev.ASAP.Application.Abstractions.Identity;
using ITMO.Dev.ASAP.Application.DataAccess.Queries;
using ITMO.Dev.ASAP.Domain.Study;

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

    public bool CanManageStudents => true;

    public bool CanUpdateAllDeadlines => true;

    public bool HasAccessToSubjectCourse(SubjectCourse subjectCourse)
    {
        return true;
    }

    public SubjectQuery.Builder FilterAvailableSubjects(SubjectQuery.Builder queryBuilder)
    {
        return queryBuilder;
    }

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