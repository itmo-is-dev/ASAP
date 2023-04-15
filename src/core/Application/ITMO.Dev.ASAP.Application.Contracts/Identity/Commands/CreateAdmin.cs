using MediatR;

namespace ITMO.Dev.ASAP.Application.Contracts.Identity.Commands;

internal static class CreateAdmin
{
    public record Command(string Username, string Password) : IRequest;
}