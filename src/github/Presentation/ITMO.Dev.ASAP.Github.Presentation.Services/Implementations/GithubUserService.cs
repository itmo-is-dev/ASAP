using ITMO.Dev.ASAP.Github.Application.Contracts.Users.Queries;
using ITMO.Dev.ASAP.Github.Application.Dto.Users;
using ITMO.Dev.ASAP.Github.Presentation.Contracts.Services;
using MediatR;

namespace ITMO.Dev.ASAP.Github.Presentation.Services.Implementations;

internal class GithubUserService : IGithubUserService
{
    private readonly IMediator _mediator;

    public GithubUserService(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<GithubUserDto?> FindByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var query = new FindUserById.Query(id);
        FindUserById.Response response = await _mediator.Send(query, cancellationToken);

        return response.User;
    }

    public async Task<IReadOnlyCollection<GithubUserDto>> FindByIdsAsync(
        IEnumerable<Guid> ids,
        CancellationToken cancellationToken)
    {
        var query = new FindUsersByIds.Query(ids);
        FindUsersByIds.Response response = await _mediator.Send(query, cancellationToken);

        return response.Users;
    }
}