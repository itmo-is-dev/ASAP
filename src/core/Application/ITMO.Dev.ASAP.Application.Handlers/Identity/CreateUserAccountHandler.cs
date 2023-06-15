using ITMO.Dev.ASAP.Application.Abstractions.Identity;
using ITMO.Dev.ASAP.Application.Common.Exceptions;
using ITMO.Dev.ASAP.Application.DataAccess;
using ITMO.Dev.ASAP.Application.DataAccess.Queries;
using ITMO.Dev.ASAP.Common.Exceptions;
using ITMO.Dev.ASAP.Domain.Users;
using MediatR;
using static ITMO.Dev.ASAP.Application.Contracts.Identity.Commands.CreateUserAccount;

namespace ITMO.Dev.ASAP.Application.Handlers.Identity;

internal class CreateUserAccountHandler : IRequestHandler<Command>
{
    private readonly IPersistenceContext _context;
    private readonly ICurrentUser _currentUser;
    private readonly IAuthorizationService _authorizationService;

    public CreateUserAccountHandler(
        IPersistenceContext context,
        ICurrentUser currentUser,
        IAuthorizationService authorizationService)
    {
        _context = context;
        _currentUser = currentUser;
        _authorizationService = authorizationService;
    }

    public async Task Handle(Command request, CancellationToken cancellationToken)
    {
        var query = UserQuery.Build(x => x.WithId(request.UserId));

        bool userExists = await _context.Users.QueryAsync(query, cancellationToken).AnyAsync(cancellationToken);

        if (userExists is false)
            throw EntityNotFoundException.For<User>(request.UserId);

        if (_currentUser.CanCreateUserWithRole(request.RoleName) is false)
            throw new AccessDeniedException($"User {_currentUser.Id} can't create user with role {request.RoleName}");

        await _authorizationService.CreateUserAsync(
            request.UserId,
            request.Username,
            request.Password,
            request.RoleName,
            cancellationToken);
    }
}