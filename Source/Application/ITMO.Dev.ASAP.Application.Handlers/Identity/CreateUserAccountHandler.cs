using ITMO.Dev.ASAP.Application.Abstractions.Identity;
using ITMO.Dev.ASAP.Common.Exceptions;
using ITMO.Dev.ASAP.Core.Users;
using ITMO.Dev.ASAP.DataAccess.Abstractions;
using ITMO.Dev.ASAP.Identity.Entities;
using ITMO.Dev.ASAP.Identity.Extensions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using static ITMO.Dev.ASAP.Application.Contracts.Identity.Commands.CreateUserAccount;

namespace ITMO.Dev.ASAP.Application.Handlers.Identity;

internal class CreateUserAccountHandler : IRequestHandler<Command>
{
    private readonly IDatabaseContext _context;
    private readonly ICurrentUser _currentUser;
    private readonly UserManager<AsapIdentityUser> _userManager;

    public CreateUserAccountHandler(
        IDatabaseContext context,
        ICurrentUser currentUser,
        UserManager<AsapIdentityUser> userManager)
    {
        _context = context;
        _currentUser = currentUser;
        _userManager = userManager;
    }

    public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
    {
        if (_userManager.Users.Any(x => x.UserName.Equals(request.Username)))
            throw new AsapIdentityException($"User with username '{request.Username}' already exists");

        if (_context.Users.Any(x => x.Id.Equals(request.UserId)) is false)
            throw EntityNotFoundException.For<User>(request.UserId);

        if (_currentUser.CanCreateUserWithRole(request.RoleName))
            throw new AsapIdentityException($"User {_currentUser.Id} can't create user with role {request.RoleName}");

        var user = new AsapIdentityUser()
        {
            Id = request.UserId,
            UserName = request.Username,
            SecurityStamp = Guid.NewGuid().ToString(),
        };

        await _userManager.CreateOrElseThrowAsync(user, request.Password);
        await _userManager.AddToRoleAsync(user, request.RoleName);

        return Unit.Value;
    }
}