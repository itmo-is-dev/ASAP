using ITMO.Dev.ASAP.Github.Application.Dto.Users;
using ITMO.Dev.ASAP.Github.Domain.Users;

namespace ITMO.Dev.ASAP.Github.Application.Mapping;

public static class GithubUserMapping
{
    public static GithubUserDto ToDto(this GithubUser user)
        => new GithubUserDto(user.Id, user.Username);
}