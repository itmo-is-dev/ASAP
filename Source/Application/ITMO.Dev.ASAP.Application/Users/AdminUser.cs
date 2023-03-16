using ITMO.Dev.ASAP.Application.Abstractions.Identity;

namespace ITMO.Dev.ASAP.Application.Users;

internal class AdminUser : ICurrentUser
{
    public AdminUser(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; }

    public bool CanCreateUserWithRole(string roleName)
    {
        return true;
    }

    public bool CanChangeUserRole(string currentRoleName, string newRoleName)
    {
        return true;
    }
}