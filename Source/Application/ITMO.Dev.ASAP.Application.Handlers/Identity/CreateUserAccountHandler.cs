using ITMO.Dev.ASAP.Common.Exceptions;
using ITMO.Dev.ASAP.DataAccess.Abstractions;
using ITMO.Dev.ASAP.Identity.Entities;
using ITMO.Dev.ASAP.Identity.Extensions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using static ITMO.Dev.ASAP.Application.Contracts.Identity.Commands.CreateUserAccount;

namespace ITMO.Dev.ASAP.Application.Handlers.Identity;

internal class CreateUserAccountHandler : IRequestHandler<Command>
{
    private readonly IDatabaseContext _context;
    private readonly UserManager<AsapIdentityUser> _userManager;

    public CreateUserAccountHandler(
        IDatabaseContext context,
        RoleManager<AsapIdentityRole> roleManager,
        UserManager<AsapIdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
    {
        if (_userManager.Users.Any(x => x.UserName.Equals(request.Username)) is false)
            throw new RegistrationFailedException($"User with username '{request.Username}' already exists");

        if (_context.Users.Any(x => x.Id.Equals(request.UserId)) is false)
            throw new RegistrationFailedException($"User with id {request.UserId} already exists");

        var user = new AsapIdentityUser()
        {
            Id = request.UserId,
            UserName = request.Username,
            SecurityStamp = Guid.NewGuid().ToString(),
        };

        await _userManager.CreateOrElseThrowAsync(user, request.Password);

        return Unit.Value;
    }
}