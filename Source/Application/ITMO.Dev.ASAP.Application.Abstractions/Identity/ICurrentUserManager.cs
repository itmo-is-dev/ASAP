using System.Security.Claims;

namespace ITMO.Dev.ASAP.Application.Abstractions.Identity;

public interface ICurrentUserManager
{
    void Authenticate(ClaimsPrincipal principal);
}