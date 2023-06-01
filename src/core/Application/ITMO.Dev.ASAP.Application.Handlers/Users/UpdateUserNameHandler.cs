using ITMO.Dev.ASAP.Application.DataAccess;
using ITMO.Dev.ASAP.Application.DataAccess.Extensions;
using ITMO.Dev.ASAP.Domain.Users;
using MediatR;
using static ITMO.Dev.ASAP.Application.Contracts.Students.Commands.UpdateUserName;

namespace ITMO.Dev.ASAP.Application.Handlers.Users;

internal class UpdateUserNameHandler : IRequestHandler<Command>
{
    private readonly IDatabaseContext _context;

    public UpdateUserNameHandler(IDatabaseContext context)
    {
        _context = context;
    }

    public async Task Handle(Command request, CancellationToken cancellationToken)
    {
        User user = await _context.Users.GetByIdAsync(request.UserId, cancellationToken);

        user.FirstName = request.FirstName;
        user.MiddleName = request.MiddleName;
        user.LastName = request.LastName;

        _context.Users.Update(user);
        await _context.SaveChangesAsync(cancellationToken);
    }
}