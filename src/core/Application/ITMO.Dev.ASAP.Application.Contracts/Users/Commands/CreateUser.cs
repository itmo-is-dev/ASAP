using ITMO.Dev.ASAP.Application.Dto.Users;
using MediatR;

namespace ITMO.Dev.ASAP.Application.Contracts.Users.Commands;

public static class CreateUser
{
    public record Command(string FirstName, string MiddleName, string LastName) : IRequest<Response>;

    public record Response(UserDto User);
}