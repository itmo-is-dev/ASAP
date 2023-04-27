using ITMO.Dev.ASAP.Application.Abstractions.Identity;
using ITMO.Dev.ASAP.Application.Dto.Study;
using ITMO.Dev.ASAP.DataAccess.Abstractions;
using ITMO.Dev.ASAP.Domain.Study;
using ITMO.Dev.ASAP.Mapping.Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static ITMO.Dev.ASAP.Application.Contracts.Study.Subjects.Queries.GetSubjects;

namespace ITMO.Dev.ASAP.Application.Handlers.Study.Subjects;

internal class GetSubjectsHandler : IRequestHandler<Query, Response>
{
    private readonly IDatabaseContext _context;
    private readonly ICurrentUser _currentUser;

    public GetSubjectsHandler(IDatabaseContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        List<Subject> subjects = await _currentUser
            .FilterAvailableSubjects(_context.Subjects)
            .ToListAsync(cancellationToken);

        SubjectDto[] dto = subjects
            .Select(x => x.ToDto())
            .ToArray();

        return new Response(dto);
    }
}