using MediatR;

namespace ITMO.Dev.ASAP.Application.Contracts.Identity.Commands;

internal static class ChangeUserRole
{
    public record struct Command(string Username, string UserRole) : IRequest;
}