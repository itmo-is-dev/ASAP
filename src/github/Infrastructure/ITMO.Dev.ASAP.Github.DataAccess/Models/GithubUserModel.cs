using ITMO.Dev.ASAP.Github.Domain.Users;

namespace ITMO.Dev.ASAP.Github.DataAccess.Models;

public record GithubUserModel(Guid Id, string Username)
{
    public GithubUser ToEntity()
    {
        return new GithubUser(Id, Username);
    }
}