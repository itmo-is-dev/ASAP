using ITMO.Dev.ASAP.Application.Abstractions.Identity;
using ITMO.Dev.ASAP.Common.Exceptions;
using ITMO.Dev.ASAP.Identity.Entities;
using System.Security.Claims;

namespace ITMO.Dev.ASAP.Application.Users;

public class CurrentUserProxy : ICurrentUser, ICurrentUserManager
{
    private ICurrentUser _user = new AnonymousUser();

    public Guid Id => _user.Id;

    public bool CanChangeRole(string currentRoleName, string newRoleName)
    {
        return _user.CanChangeRole(currentRoleName, newRoleName);
    }

    public void Authenticate(ClaimsPrincipal principal)
    {
        string[] roles = principal.Claims
            .Where(x => x.Type.Equals(ClaimTypes.Role, StringComparison.OrdinalIgnoreCase))
            .Select(x => x.Value)
            .ToArray();

        string nameIdentifier = principal.Claims
            .Single(x => x.Type.Equals(ClaimTypes.NameIdentifier, StringComparison.OrdinalIgnoreCase))
            .Value;

        if (!Guid.TryParse(nameIdentifier, out Guid id))
        {
            throw new UnauthorizedException("Failed to parse user NameIdentifier to Guid");
        }

        if (roles.Contains(AsapIdentityRole.AdminRoleName))
        {
            _user = new AdminUser(id);
        }
        else if (roles.Contains(AsapIdentityRole.ModeratorRoleName))
        {
            _user = new ModeratorUser(id);
        }
        else if (roles.Contains(AsapIdentityRole.MentorRoleName))
        {
            _user = new MentorUser(id);
        }
        else
        {
            _user = new AnonymousUser();
        }
    }
}