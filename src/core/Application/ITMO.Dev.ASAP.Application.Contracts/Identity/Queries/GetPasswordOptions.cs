using ITMO.Dev.ASAP.Application.Dto.Identity;
using MediatR;

namespace ITMO.Dev.ASAP.Application.Contracts.Identity.Queries;

internal static class GetPasswordOptions
{
    public record Query : IRequest<Response>;

    public record Response(PasswordOptionsDto PasswordOptions);
}