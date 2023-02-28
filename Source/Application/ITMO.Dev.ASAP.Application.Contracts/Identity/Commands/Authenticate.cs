using MediatR;
using System.Security.Claims;

namespace ITMO.Dev.ASAP.Application.Contracts.Identity.Commands;

internal static class Authenticate
{
    public record Command(ClaimsPrincipal Principal) : IRequest;
}