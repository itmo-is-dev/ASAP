using ITMO.Dev.ASAP.Application.Abstractions.Identity;
using ITMO.Dev.ASAP.Application.Common.Exceptions;
using ITMO.Dev.ASAP.Application.DataAccess;
using ITMO.Dev.ASAP.Application.Specifications;
using ITMO.Dev.ASAP.Domain.UserAssociations;
using ITMO.Dev.ASAP.Domain.Users;
using MediatR;
using static ITMO.Dev.ASAP.Application.Contracts.Users.Commands.UpdateUserUniversityId;

namespace ITMO.Dev.ASAP.Application.Handlers.Users;

internal class UpdateUserUniversityIdHandler : IRequestHandler<Command>
{
    private readonly IPersistenceContext _context;
    private readonly ICurrentUser _currentUser;

    public UpdateUserUniversityIdHandler(
        IPersistenceContext context,
        ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task Handle(Command request, CancellationToken cancellationToken)
    {
        if (_currentUser.CanManageStudents is false)
            throw UserHasNotAccessException.AccessViolation(_currentUser.Id);

        User user = await _context.Users.GetByIdAsync(request.UserId, cancellationToken);
        IsuUserAssociation? association = user.FindAssociation<IsuUserAssociation>();

        if (association is null)
        {
            association = IsuUserAssociation.CreateAndAttach(Guid.NewGuid(), user, request.UniversityId);
            _context.UserAssociations.Add(association);
        }
        else
        {
            association.UniversityId = request.UniversityId;
            _context.UserAssociations.Update(association);
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}