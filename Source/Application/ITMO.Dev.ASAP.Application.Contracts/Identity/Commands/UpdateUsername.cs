using MediatR;

namespace ITMO.Dev.ASAP.Application.Contracts.Identity.Commands;

internal static class UpdateUsername
{
    public record Command(string Username) : IRequest;
}