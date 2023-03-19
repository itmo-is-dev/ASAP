using ITMO.Dev.ASAP.Application.Dto.Identity;
using ITMO.Dev.ASAP.Identity.Tools;
using Microsoft.Extensions.Configuration;

namespace ITMO.Dev.ASAP.Mapping.Mappings;

public static class PasswordOptionsMapping
{
    public static PasswordOptionsDto ToPasswordOptionsDto(this IConfigurationSection identityConfigurationSection)
    {
        IdentityConfigurationPassword? identityConfigurationPassword = identityConfigurationSection
            .Get<IdentityConfigurationPassword>();

        return new PasswordOptionsDto(
            identityConfigurationPassword.RequireDigit,
            identityConfigurationPassword.RequireLowercase,
            identityConfigurationPassword.RequireNonAlphanumeric,
            identityConfigurationPassword.RequireUppercase,
            identityConfigurationPassword.RequiredLength,
            identityConfigurationPassword.RequiredUniqueChars);
    }
}