using ITMO.Dev.ASAP.Github.Application.Dto.Users;
using MediatR;

namespace ITMO.Dev.ASAP.Github.Application.Contracts.Users.Commands;

public static class UpdateGithubUser
{
    public record Command(Guid UserId, string GithubUsername) : IRequest<Response>;

    public record Response(GithubUserDto User);
}