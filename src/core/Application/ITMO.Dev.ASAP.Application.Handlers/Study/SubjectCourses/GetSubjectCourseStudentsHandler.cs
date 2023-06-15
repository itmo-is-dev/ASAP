using ITMO.Dev.ASAP.Application.DataAccess;
using ITMO.Dev.ASAP.Application.Specifications;
using ITMO.Dev.ASAP.Domain.Students;
using MediatR;
using static ITMO.Dev.ASAP.Application.Contracts.Study.SubjectCourses.Queries.GetSubjectCourseStudents;

namespace ITMO.Dev.ASAP.Application.Handlers.Study.SubjectCourses;

internal class GetSubjectCourseStudentsHandler : IRequestHandler<Query, Response>
{
    private readonly IPersistenceContext _context;

    public GetSubjectCourseStudentsHandler(IPersistenceContext context)
    {
        _context = context;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        IReadOnlyCollection<Student> students = await _context.Students
            .GetStudentsBySubjectCourseIdAsync(request.SubjectCourseIds, cancellationToken)
            .ToArrayAsync(cancellationToken);

        Guid[] ids = students.Select(x => x.UserId).ToArray();

        return new Response(ids);
    }
}