namespace ITMO.Dev.ASAP.Application.Abstractions.Identity;

public interface ICurrentUser
{
    Guid Id { get; }

    bool CanChangeRole(string currentRoleName, string newRoleName);
}