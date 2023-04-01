using ITMO.Dev.ASAP.Application.Abstractions.Identity;
using ITMO.Dev.ASAP.Common.Exceptions;
using ITMO.Dev.ASAP.Identity.Abstractions.Entities;
using ITMO.Dev.ASAP.Identity.Abstractions.Services;
using MediatR;
using static ITMO.Dev.ASAP.Application.Contracts.Identity.Commands.UpdateUsername;

namespace ITMO.Dev.ASAP.Application.Handlers.Identity;

internal class UpdateUsernameHandler : IRequestHandler<Command>
{
    private readonly ICurrentUser _currentUser;
    private readonly IIdentitySetvice _identitySetvice;

    public UpdateUsernameHandler(ICurrentUser currentUser, IIdentitySetvice identitySetvice)
    {
        _currentUser = currentUser;
        _identitySetvice = identitySetvice;
    }

    public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
    {
        AsapIdentityUser user = await _identitySetvice.GetUserByIdAsync(_currentUser.Id, cancellationToken);

        if (user.UserName?.Equals(request.Username, StringComparison.Ordinal) is true)
            throw new UpdateUsernameFailedException("the old username is the same as the new one");

        await _identitySetvice.UpdateUserNameAsync(user, request.Username, cancellationToken);

        return Unit.Value;
    }
}