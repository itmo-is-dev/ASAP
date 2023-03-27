using ITMO.Dev.ASAP.Application.Dto.Querying;
using ITMO.Dev.ASAP.Application.Dto.Users;
using MediatR;

namespace ITMO.Dev.ASAP.Application.Contracts.Users.Queries;

internal static class GetUserIdentityInfos
{
    public record Query(QueryConfiguration<UserQueryParameter> QueryConfiguration, int Page) : IRequest<Response>;

    public record Response(IReadOnlyCollection<UserIdentityInfoDto> Users, int PageCount);
}