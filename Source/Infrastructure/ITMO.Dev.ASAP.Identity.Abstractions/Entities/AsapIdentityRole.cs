using Microsoft.AspNetCore.Identity;

namespace ITMO.Dev.ASAP.Identity.Abstractions.Entities;

public class AsapIdentityRole : IdentityRole<Guid>
{
    public const string AdminRoleName = "Admin";
    public const string ModeratorRoleName = "Moderator";
    public const string MentorRoleName = "Mentor";

    public AsapIdentityRole(string roleName)
        : base(roleName) { }

    protected AsapIdentityRole() { }
}