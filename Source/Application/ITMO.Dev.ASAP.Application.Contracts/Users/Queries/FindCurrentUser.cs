using ITMO.Dev.ASAP.Application.Dto.Users;
using MediatR;

namespace ITMO.Dev.ASAP.Application.Contracts.Users.Queries;

internal static class FindCurrentUser
{
    public record Query : IRequest<Response>;

    public record Response(UserDto? User);
}