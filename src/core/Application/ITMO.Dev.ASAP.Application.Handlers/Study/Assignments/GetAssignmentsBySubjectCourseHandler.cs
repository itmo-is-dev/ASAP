using ITMO.Dev.ASAP.Application.DataAccess;
using ITMO.Dev.ASAP.Application.DataAccess.Queries;
using ITMO.Dev.ASAP.Application.Dto.Study;
using ITMO.Dev.ASAP.Domain.Study.Assignments;
using ITMO.Dev.ASAP.Mapping.Mappings;
using MediatR;
using static ITMO.Dev.ASAP.Application.Contracts.Study.Assignments.Queries.GetAssignmentsBySubjectCourse;

namespace ITMO.Dev.ASAP.Application.Handlers.Study.Assignments;

internal class GetAssignmentsBySubjectCourseHandler : IRequestHandler<Query, Response>
{
    private readonly IPersistenceContext _context;

    public GetAssignmentsBySubjectCourseHandler(IPersistenceContext context)
    {
        _context = context;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        var query = AssignmentQuery.Build(x => x.WithSubjectCourseId(request.SubjectCourseId));

        IAsyncEnumerable<Assignment> assignments = _context.Assignments
            .QueryAsync(query, cancellationToken);

        AssignmentDto[] dto = await assignments
            .Select(x => x.ToDto(request.SubjectCourseId))
            .ToArrayAsync(cancellationToken);

        return new Response(dto);
    }
}