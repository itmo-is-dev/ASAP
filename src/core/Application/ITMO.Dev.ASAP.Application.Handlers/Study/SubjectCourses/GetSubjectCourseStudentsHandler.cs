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
        IAsyncEnumerable<Student> students = _context.Students
            .GetStudentsBySubjectCourseIdAsync(request.SubjectCourseIds, cancellationToken);

        Guid[] ids = await students.Select(x => x.UserId).ToArrayAsync(cancellationToken);

        return new Response(ids);
    }
}