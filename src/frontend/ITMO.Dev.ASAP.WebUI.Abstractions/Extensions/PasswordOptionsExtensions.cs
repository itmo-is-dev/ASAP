using ITMO.Dev.ASAP.Application.Dto.Identity;

namespace ITMO.Dev.ASAP.WebUI.Abstractions.Extensions;

public static class PasswordOptionsExtensions
{
    public static bool TryValidate(this PasswordOptionsDto options, string value, out string? reason)
    {
        if (options.RequireDigit && value.Any(char.IsDigit) is false)
        {
            reason = "Password requires digits";
            return false;
        }

        if (options.RequireLowercase && value.Any(char.IsLower) is false)
        {
            reason = "Password requires lowercase letters";
            return false;
        }

        if (options.RequireNonAlphanumeric && value.Any(char.IsLetterOrDigit) is false)
        {
            reason = "Password requires non alphanumeric characters";
            return false;
        }

        if (options.RequireUppercase && value.Any(char.IsUpper) is false)
        {
            reason = "Password requires uppercase letters";
            return false;
        }

        if (value.Length < options.RequiredLength)
        {
            reason = $"Password must be at least {options.RequiredLength} characters long";
            return false;
        }

        if (value.GroupBy(x => x).Count() < options.RequiredUniqueChars)
        {
            reason = "Password requires unique characters";
            return false;
        }

        reason = null;
        return true;
    }
}