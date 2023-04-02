using ITMO.Dev.ASAP.Identity.Abstractions.Models;
using Microsoft.AspNetCore.Identity;

namespace ITMO.Dev.ASAP.Identity.Entities;

internal class AsapIdentityUser : IdentityUser<Guid>
{
    public AsapIdentityUserDto ToDto()
    {
        return new AsapIdentityUserDto(Id, UserName);
    }
}