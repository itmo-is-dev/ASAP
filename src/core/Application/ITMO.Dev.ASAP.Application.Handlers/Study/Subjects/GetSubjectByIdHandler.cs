using ITMO.Dev.ASAP.Application.Abstractions.Identity;
using ITMO.Dev.ASAP.Application.Common.Exceptions;
using ITMO.Dev.ASAP.DataAccess.Abstractions;
using ITMO.Dev.ASAP.DataAccess.Abstractions.Extensions;
using ITMO.Dev.ASAP.Domain.Study;
using ITMO.Dev.ASAP.Mapping.Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static ITMO.Dev.ASAP.Application.Contracts.Study.Subjects.Queries.GetSubjectById;

namespace ITMO.Dev.ASAP.Application.Handlers.Study.Subjects;

internal class GetSubjectByIdHandler : IRequestHandler<Query, Response>
{
    private readonly IDatabaseContext _context;
    private readonly ICurrentUser _currentUser;

    public GetSubjectByIdHandler(IDatabaseContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        Subject subject = await _context.Subjects
            .Include(x => x.Courses)
            .GetByIdAsync(request.Id, cancellationToken);

        if (_currentUser.HasAccessToSubject(subject) is false)
            throw UserHasNotAccessException.AccessViolation(_currentUser.Id);

        return subject.Courses.Any(_currentUser.HasAccessToSubjectCourse)
            ? new Response(subject.ToDto())
            : throw UserHasNotAccessException.EmptyAvailableList(_currentUser.Id);
    }
}