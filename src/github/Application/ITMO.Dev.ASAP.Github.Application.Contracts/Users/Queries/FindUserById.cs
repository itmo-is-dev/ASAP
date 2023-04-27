using ITMO.Dev.ASAP.Github.Application.Dto.Users;
using MediatR;

namespace ITMO.Dev.ASAP.Github.Application.Contracts.Users.Queries;

public static class FindUserById
{
    public record Query(Guid Id) : IRequest<Response>;

    public record Response(GithubUserDto? User);
}