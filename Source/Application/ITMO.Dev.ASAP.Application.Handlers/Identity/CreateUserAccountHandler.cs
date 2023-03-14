using ITMO.Dev.ASAP.Common.Exceptions;
using ITMO.Dev.ASAP.DataAccess.Abstractions;
using ITMO.Dev.ASAP.Identity.Entities;
using ITMO.Dev.ASAP.Identity.Extensions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
        if (!await _userManager.Users.AnyAsync(x => x.UserName.Equals(request.Username), cancellationToken))
            throw new RegistrationFailedException($"User with username '{request.Username}' already exists");

        if (!await _context.Users.AnyAsync(x => x.Id.Equals(request.UserId), cancellationToken))
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