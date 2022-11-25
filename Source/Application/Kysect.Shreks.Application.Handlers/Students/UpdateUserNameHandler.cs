﻿using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.DataAccess.Abstractions.Extensions;
using MediatR;
using static Kysect.Shreks.Application.Contracts.Students.Commands.UpdateUserName;

namespace Kysect.Shreks.Application.Handlers.Students;

public class UpdateUserNameHandler : IRequestHandler<Command, Response>
{
    private readonly IShreksDatabaseContext _context;

    public UpdateUserNameHandler(IShreksDatabaseContext context)
    {
        _context = context;
    }

    public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
    {
        var user = await _context.Users.GetByIdAsync(request.UserId, cancellationToken: cancellationToken);

        user.FirstName = request.FirstName;
        user.MiddleName = request.MiddleName;
        user.LastName = request.LastName;

        _context.Users.Update(user);
        await _context.SaveChangesAsync(cancellationToken);

        return new Response();
    }
}