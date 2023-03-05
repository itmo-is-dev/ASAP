using MediatR;

namespace ITMO.Dev.ASAP.Application.Contracts.Identity.Commands;

internal static class CreateUserAccount
{
    public record struct Command(Guid UserId, string Username, string Password) : IRequest;
}
