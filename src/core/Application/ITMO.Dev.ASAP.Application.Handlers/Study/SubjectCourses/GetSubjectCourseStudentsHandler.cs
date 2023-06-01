using ITMO.Dev.ASAP.Application.DataAccess;
using ITMO.Dev.ASAP.Application.Specifications;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static ITMO.Dev.ASAP.Application.Contracts.Study.SubjectCourses.Queries.GetSubjectCourseStudents;

namespace ITMO.Dev.ASAP.Application.Handlers.Study.SubjectCourses;

internal class GetSubjectCourseStudentsHandler : IRequestHandler<Query, Response>
{
    private readonly IDatabaseContext _context;

    public GetSubjectCourseStudentsHandler(IDatabaseContext context)
    {
        _context = context;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        List<Guid> students = await _context.SubjectCourses
            .WithId(request.SubjectCourseIds)
            .GetAllStudents()
            .Select(x => x.UserId)
            .ToListAsync(cancellationToken);

        return new Response(students);
    }
}