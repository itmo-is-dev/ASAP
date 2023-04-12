using ITMO.Dev.ASAP.Application.Dto.Identity;
using Microsoft.AspNetCore.Identity;

namespace ITMO.Dev.ASAP.Identity.Entities;

internal class AsapIdentityUser : IdentityUser<Guid>
{
    public IdentityUserDto ToDto()
    {
        return new IdentityUserDto(Id, UserName);
    }
}