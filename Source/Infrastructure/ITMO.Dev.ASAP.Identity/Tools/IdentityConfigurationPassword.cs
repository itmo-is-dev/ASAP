namespace ITMO.Dev.ASAP.Identity.Tools;

public class IdentityConfigurationPassword
{
    public bool RequireDigit { get; init; } = true;
    public bool RequireLowercase { get; init; } = true;
    public bool RequireNonAlphanumeric { get; init; } = true;
    public bool RequireUppercase { get; init; } = true;
    public int RequiredLength { get; init; } = 6;
    public int RequiredUniqueChars { get; init; } = 1;
}