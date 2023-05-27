using ITMO.Dev.ASAP.Application.Abstractions.Identity;
using ITMO.Dev.ASAP.Application.Contracts.Tools;
using ITMO.Dev.ASAP.Application.DataAccess;
using ITMO.Dev.ASAP.Application.Dto.Identity;
using ITMO.Dev.ASAP.Application.Dto.Querying;
using ITMO.Dev.ASAP.Application.Dto.Users;
using ITMO.Dev.ASAP.Application.Queries;
using ITMO.Dev.ASAP.Domain.Users;
using ITMO.Dev.ASAP.Mapping.Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using static ITMO.Dev.ASAP.Application.Contracts.Users.Queries.GetUserIdentityInfos;

namespace ITMO.Dev.ASAP.Application.Handlers.Users;

internal class GitUserIdentityInfosHandler : IRequestHandler<Query, Response>
{
    private readonly IDatabaseContext _context;
    private readonly IAuthorizationService _authorizationService;
    private readonly PaginationConfiguration _paginationConfiguration;
    private readonly IEntityQuery<User, UserQueryParameter> _userQuery;

    public GitUserIdentityInfosHandler(
        IDatabaseContext context,
        IAuthorizationService authorizationService,
        IOptions<PaginationConfiguration> paginationConfiguration,
        IEntityQuery<User, UserQueryParameter> userQuery)
    {
        _context = context;
        _authorizationService = authorizationService;
        _paginationConfiguration = paginationConfiguration.Value;
        _userQuery = userQuery;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        IQueryable<User> query = _context.Users;
        query = _userQuery.Apply(query, request.QueryConfiguration);

        int userCount = await query.CountAsync(cancellationToken);
        int pageCount = (int)Math.Ceiling((double)userCount / _paginationConfiguration.PageSize);

        if (request.Page >= pageCount)
            return new Response(Array.Empty<UserIdentityInfoDto>(), pageCount);

        List<User> users = await query
            .OrderBy(x => x.LastName)
            .Skip(request.Page * _paginationConfiguration.PageSize)
            .Take(_paginationConfiguration.PageSize)
            .ToListAsync(cancellationToken);

        IEnumerable<Guid> userIds = users.Select(x => x.Id);

        IEnumerable<IdentityUserDto> identityUsers =
            await _authorizationService.GetUsersByIdsAsync(userIds, cancellationToken);

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