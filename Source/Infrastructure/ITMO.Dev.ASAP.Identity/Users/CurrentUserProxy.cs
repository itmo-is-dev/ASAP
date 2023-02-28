using ITMO.Dev.ASAP.Application.Abstractions.Identity;
using ITMO.Dev.ASAP.Identity.Entities;
using System.Security.Claims;

namespace ITMO.Dev.ASAP.Identity.Users;

public class CurrentUserProxy : ICurrentUser, ICurrentUserManager
{
    private ICurrentUser _user = new AnonymousUser(Guid.Empty);

    public Guid Id => _user.Id;

    public void Authenticate(ClaimsPrincipal principal)
    {
        string[] roles = principal.Claims
            .Where(x => x.Type.Equals(ClaimTypes.Role, StringComparison.OrdinalIgnoreCase))
            .Select(x => x.Value)
            .ToArray();

        Guid.TryParse(
            principal.Claims
                .Single(x => x.Type.Equals(ClaimTypes.NameIdentifier, StringComparison.OrdinalIgnoreCase))
                .Value,
            out Guid id);

        if (roles.Any(x => x == AsapIdentityRole.AdminRoleName))
        {
            _user = new AdminUser(id);
        }
        else if (roles.Any(x => x == AsapIdentityRole.ModeratorRoleName))
        {
            _user = new ModeratorUser(id);
        }
        else if (roles.Any(x => x == AsapIdentityRole.MentorRoleName))
        {
            _user = new MentorUser(id);
        }
        else
        {
            _user = new AnonymousUser(id);
        }
    }
}