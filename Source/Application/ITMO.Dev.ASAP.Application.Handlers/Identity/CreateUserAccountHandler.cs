using ITMO.Dev.ASAP.Application.Abstractions.Identity;
using ITMO.Dev.ASAP.Common.Exceptions;
using ITMO.Dev.ASAP.Core.Users;
using ITMO.Dev.ASAP.DataAccess.Abstractions;
using ITMO.Dev.ASAP.Identity.Abstractions.Services;
using MediatR;
using static ITMO.Dev.ASAP.Application.Contracts.Identity.Commands.CreateUserAccount;

namespace ITMO.Dev.ASAP.Application.Handlers.Identity;

internal class CreateUserAccountHandler : IRequestHandler<Command>
{
    private readonly IDatabaseContext _context;
    private readonly ICurrentUser _currentUser;
    private readonly IIdentitySetvice _identitySetvice;

    public CreateUserAccountHandler(
        IDatabaseContext context,
        ICurrentUser currentUser,
        IIdentitySetvice identitySetvice)
    {
        _context = context;
        _currentUser = currentUser;
        _identitySetvice = identitySetvice;
    }

    public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
    {
        if (_context.Users.Any(x => x.Id.Equals(request.UserId)) is false)
            throw EntityNotFoundException.For<User>(request.UserId);

        if (_currentUser.CanCreateUserWithRole(request.RoleName) is false)
            throw new AsapIdentityException($"User {_currentUser.Id} can't create user with role {request.RoleName}");

        await _identitySetvice.CreateUserAsync(
            request.UserId,
            request.Username,
            request.Password,
            request.RoleName,
            cancellationToken);

        return Unit.Value;
    }
}