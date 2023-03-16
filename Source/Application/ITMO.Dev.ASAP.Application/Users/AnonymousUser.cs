using ITMO.Dev.ASAP.Application.Abstractions.Identity;
using ITMO.Dev.ASAP.Common.Exceptions;

namespace ITMO.Dev.ASAP.Application.Users;

internal class AnonymousUser : ICurrentUser
{
#pragma warning disable CA1065
    public Guid Id => throw new UnauthorizedException("Tried to access anonymous user Id");

    public bool CanUpdateAllDeadlines => false;

    public bool CanCreateUserWithRole(string roleName)
    {
        throw new UnauthorizedException("Tried to access anonymous user Id");
    }

    public bool CanChangeUserRole(string currentRoleName, string newRoleName)
    {
        throw new UnauthorizedException("Tried to access anonymous user Id");
    }
#pragma warning restore CA1065
}