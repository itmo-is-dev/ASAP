using ITMO.Dev.ASAP.Application.Dto.Users;
using MediatR;

namespace ITMO.Dev.ASAP.Application.Contracts.Users.Queries;

public static class GetUserById
{
    public record Query(Guid UserId) : IRequest<Response>;

    public record Response(UserDto User);
}