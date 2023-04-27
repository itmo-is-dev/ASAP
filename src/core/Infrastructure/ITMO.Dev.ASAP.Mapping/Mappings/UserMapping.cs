using ITMO.Dev.ASAP.Application.Dto.Users;
using User = ITMO.Dev.ASAP.Domain.Users.User;

namespace ITMO.Dev.ASAP.Mapping.Mappings;

public static class UserMapping
{
    public static UserDto ToDto(this User user)
    {
        return new UserDto(user.Id, user.FirstName, user.MiddleName, user.LastName);
    }
}