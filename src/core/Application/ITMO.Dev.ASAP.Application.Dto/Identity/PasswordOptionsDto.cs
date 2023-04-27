namespace ITMO.Dev.ASAP.Application.Dto.Identity;

public record PasswordOptionsDto(
    bool RequireDigit,
    bool RequireLowercase,
    bool RequireNonAlphanumeric,
    bool RequireUppercase,
    int RequiredLength,
    int RequiredUniqueChars);