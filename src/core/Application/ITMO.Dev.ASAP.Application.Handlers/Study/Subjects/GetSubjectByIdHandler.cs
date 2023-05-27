using ITMO.Dev.ASAP.Application.Abstractions.Identity;
using ITMO.Dev.ASAP.Application.Common.Exceptions;
using ITMO.Dev.ASAP.Application.DataAccess;
using ITMO.Dev.ASAP.Application.DataAccess.Queries;
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
        var query = SubjectQuery.Build(x => _currentUser.FilterAvailableSubjects(x).WithId(request.Id));

        Subject? subject = await _context.Subjects
            .QueryAsync(query, cancellationToken)
            .SingleOrDefaultAsync(cancellationToken);

        if (subject is null)
            throw UserHasNotAccessException.AccessViolation(_currentUser.Id);

        List<SubjectCourse> courses = await _context.SubjectCourses
            .Where(x => x.SubjectId.Equals(subject.Id))
            .ToListAsync(cancellationToken);

        return courses.Any(_currentUser.HasAccessToSubjectCourse)
            ? new Response(subject.ToDto())
            : throw UserHasNotAccessException.EmptyAvailableList(_currentUser.Id);
    }
}