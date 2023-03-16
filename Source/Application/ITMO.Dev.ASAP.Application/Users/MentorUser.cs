using ITMO.Dev.ASAP.Application.Abstractions.Identity;

namespace ITMO.Dev.ASAP.Application.Users;

internal class MentorUser : ICurrentUser
{
    public MentorUser(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; }

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