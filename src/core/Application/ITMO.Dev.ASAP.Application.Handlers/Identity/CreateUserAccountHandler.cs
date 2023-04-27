using ITMO.Dev.ASAP.Application.Abstractions.Identity;
using ITMO.Dev.ASAP.Application.Common.Exceptions;
using ITMO.Dev.ASAP.Common.Exceptions;
using ITMO.Dev.ASAP.DataAccess.Abstractions;
using ITMO.Dev.ASAP.Domain.Users;
using MediatR;
using static ITMO.Dev.ASAP.Application.Contracts.Identity.Commands.CreateUserAccount;

namespace ITMO.Dev.ASAP.Application.Handlers.Identity;

internal class CreateUserAccountHandler : IRequestHandler<Command>
{
    private readonly IDatabaseContext _context;
    private readonly ICurrentUser _currentUser;
    private readonly IAuthorizationService _authorizationService;

    public CreateUserAccountHandler(
        IDatabaseContext context,
        ICurrentUser currentUser,
        IAuthorizationService authorizationService)
    {
        _context = context;
        _currentUser = currentUser;
        _authorizationService = authorizationService;
    }

    public async Task Handle(Command request, CancellationToken cancellationToken)
    {
        if (_context.Users.Any(x => x.Id.Equals(request.UserId)) is false)
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