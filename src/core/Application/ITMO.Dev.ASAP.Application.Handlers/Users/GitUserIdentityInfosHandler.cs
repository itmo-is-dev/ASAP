using ITMO.Dev.ASAP.Application.Abstractions.Identity;
using ITMO.Dev.ASAP.Application.Contracts.Tools;
using ITMO.Dev.ASAP.Application.DataAccess;
using ITMO.Dev.ASAP.Application.DataAccess.Models;
using ITMO.Dev.ASAP.Application.DataAccess.Queries;
using ITMO.Dev.ASAP.Application.Dto.Identity;
using ITMO.Dev.ASAP.Application.Dto.Querying;
using ITMO.Dev.ASAP.Application.Dto.Users;
using ITMO.Dev.ASAP.Application.Queries;
using ITMO.Dev.ASAP.Domain.Users;
using ITMO.Dev.ASAP.Mapping.Mappings;
using MediatR;
using Microsoft.Extensions.Options;
using static ITMO.Dev.ASAP.Application.Contracts.Users.Queries.GetUserIdentityInfos;

namespace ITMO.Dev.ASAP.Application.Handlers.Users;

internal class GitUserIdentityInfosHandler : IRequestHandler<Query, Response>
{
    private readonly IPersistenceContext _context;
    private readonly IAuthorizationService _authorizationService;
    private readonly PaginationConfiguration _paginationConfiguration;
    private readonly IEntityQuery<UserQuery.Builder, UserQueryParameter> _userQuery;

    public GitUserIdentityInfosHandler(
        IPersistenceContext context,
        IAuthorizationService authorizationService,
        IOptions<PaginationConfiguration> paginationConfiguration,
        IEntityQuery<UserQuery.Builder, UserQueryParameter> userQuery)
    {
        _context = context;
        _authorizationService = authorizationService;
        _paginationConfiguration = paginationConfiguration.Value;
        _userQuery = userQuery;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        var queryBuilder = new UserQuery.Builder();
        queryBuilder = _userQuery.Apply(queryBuilder, request.QueryConfiguration);

        UserQuery query = queryBuilder.Build();

        long userCount = await _context.Users.CountAsync(query, cancellationToken);
        int pageCount = (int)Math.Ceiling((double)userCount / _paginationConfiguration.PageSize);

        if (request.Page >= pageCount)
            return new Response(Array.Empty<UserIdentityInfoDto>(), pageCount);

        query = queryBuilder
            .WithOrderByLastName(OrderDirection.Ascending)
            .WithCursor(request.Page * _paginationConfiguration.PageSize)
            .WithLimit(_paginationConfiguration.PageSize)
            .Build();

        User[] users = await _context.Users
            .QueryAsync(query, cancellationToken)
            .ToArrayAsync(cancellationToken);

        IEnumerable<Guid> userIds = users.Select(x => x.Id);

        IEnumerable<IdentityUserDto> identityUsers = await _authorizationService
            .GetUsersByIdsAsync(userIds, cancellationToken);

        IEnumerable<Guid> identityUserIds = identityUsers.Select(x => x.Id);

        UserIdentityInfoDto[] dto = users
            .GroupJoin(
                identityUserIds,
                x => x.Id,
                x => x,
                (x, e) => new UserIdentityInfoDto(x.ToDto(), e.Any()))
            .ToArray();

        return new Response(dto, pageCount);
    }
}