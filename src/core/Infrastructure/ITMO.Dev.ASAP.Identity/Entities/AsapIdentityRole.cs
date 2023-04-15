using Microsoft.AspNetCore.Identity;

namespace ITMO.Dev.ASAP.Identity.Entities;

internal class AsapIdentityRole : IdentityRole<Guid>
{
    public AsapIdentityRole(string roleName)
        : base(roleName) { }

    protected AsapIdentityRole() { }
}
