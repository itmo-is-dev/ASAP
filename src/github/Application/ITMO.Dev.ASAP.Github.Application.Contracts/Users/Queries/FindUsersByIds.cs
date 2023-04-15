using ITMO.Dev.ASAP.Github.Application.Dto.Users;
using MediatR;

namespace ITMO.Dev.ASAP.Github.Application.Contracts.Users.Queries;

public static class FindUsersByIds
{
    public record Query(IEnumerable<Guid> Ids) : IRequest<Response>;

    public record Response(IReadOnlyCollection<GithubUserDto> Users);
}