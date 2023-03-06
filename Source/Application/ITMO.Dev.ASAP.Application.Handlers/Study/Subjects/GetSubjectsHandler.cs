using ITMO.Dev.ASAP.Application.Abstractions.Identity;
using ITMO.Dev.ASAP.Application.Dto.Study;
using ITMO.Dev.ASAP.Core.Study;
using ITMO.Dev.ASAP.DataAccess.Abstractions;
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
        if (_currentUser.Role is UserRoleType.Anonymous)
            throw new Exception("Anonymous user cannot access information about the subjects");

        List<Subject> subjects = await _context
            .Subjects
            .ToListAsync(cancellationToken);

        if (_currentUser.Role is UserRoleType.Mentor)
        {
            subjects = subjects
                .Where(s => s.Courses.SelectMany(c => c.Mentors).Any(m => m.UserId == _currentUser.Id))
                .ToList();

            if (subjects.Count is 0)
                throw new Exception("Mentor has not any subject");
        }

        SubjectDto[] dto = subjects.Select(x => x.ToDto()).ToArray();

        return new Response(dto);
    }
}