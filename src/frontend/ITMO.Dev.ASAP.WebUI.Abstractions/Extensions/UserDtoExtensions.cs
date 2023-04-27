using ITMO.Dev.ASAP.Application.Dto.Users;

namespace ITMO.Dev.ASAP.WebUI.Abstractions.Extensions;

public static class UserDtoExtensions
{
    public static string FullName(this UserDto user)
        => $"{user.LastName} {user.FirstName} {user.MiddleName}";
}