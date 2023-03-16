namespace ITMO.Dev.ASAP.Application.Abstractions.Identity;

public interface ICurrentUser
{
    Guid Id { get; }

    bool CanUpdateAllDeadlines { get; }

    bool CanCreateUserWithRole(string roleName);

    bool CanChangeUserRole(string currentRoleName, string newRoleName);
}