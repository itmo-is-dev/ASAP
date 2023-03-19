using ITMO.Dev.ASAP.Application.Contracts.Users.Commands;
using ITMO.Dev.ASAP.Application.Contracts.Users.Queries;
using ITMO.Dev.ASAP.Application.Dto.Users;
using ITMO.Dev.ASAP.Presentation.Contracts.Services;
using MediatR;

namespace ITMO.Dev.ASAP.Presentation.Services.Implementations;

public class AsapUserService : IAsapUserService
{
    private readonly IMediator _mediator;

    public AsapUserService(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<UserDto> CreateUserAsync(
        string firstName,
        string middleName,
        string lastName,
        CancellationToken cancellationToken)
    {
        var command = new CreateUser.Command(firstName, middleName, lastName);
        CreateUser.Response response = await _mediator.Send(command, cancellationToken);

        return response.User;
    }

    public async Task<UserDto> GetByIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        var query = new GetUserById.Query(userId);
        GetUserById.Response response = await _mediator.Send(query, cancellationToken);

        return response.User;
    }
}