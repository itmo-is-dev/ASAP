using ITMO.Dev.ASAP.Application.Dto.Identity;
using Microsoft.AspNetCore.Identity;

namespace ITMO.Dev.ASAP.Mapping.Mappings;

public static class PasswordOptionsMapping
{
    public static PasswordOptionsDto ToDto(this IdentityOptions options)
    {
        return new PasswordOptionsDto(
            options.Password.RequireDigit,
            options.Password.RequireLowercase,
            options.Password.RequireNonAlphanumeric,
            options.Password.RequireUppercase,
            options.Password.RequiredLength,
            options.Password.RequiredUniqueChars);
    }
}