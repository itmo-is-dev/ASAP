using MediatR;

namespace ITMO.Dev.ASAP.Application.Contracts.Identity.Queries;

internal static class Login
{
    public record Query(string Username, string Password) : IRequest<Response>;

    public record Response(string Token);
}