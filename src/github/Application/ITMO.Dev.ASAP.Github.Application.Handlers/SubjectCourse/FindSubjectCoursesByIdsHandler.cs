using ITMO.Dev.ASAP.Github.Application.DataAccess;
using ITMO.Dev.ASAP.Github.Application.DataAccess.Queries;
using ITMO.Dev.ASAP.Github.Application.Dto.SubjectCourses;
using ITMO.Dev.ASAP.Github.Application.Mapping;
using ITMO.Dev.ASAP.Github.Domain.SubjectCourses;
using MediatR;
using static ITMO.Dev.ASAP.Github.Application.Contracts.SubjectCourses.Queries.FindSubjectCoursesByIds;

namespace ITMO.Dev.ASAP.Github.Application.Handlers.SubjectCourse;

internal class FindSubjectCoursesByIdsHandler : IRequestHandler<Query, Response>
{
    private readonly IPersistenceContext _context;

    public FindSubjectCoursesByIdsHandler(IPersistenceContext context)
    {
        _context = context;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        var query = GithubSubjectCourseQuery.Build(x => x.WithIds(request.Ids));

        IAsyncEnumerable<GithubSubjectCourse> subjectCourses = _context.SubjectCourses
            .QueryAsync(query, cancellationToken);

        List<GithubSubjectCourseDto> dto = await subjectCourses
            .Select(x => x.ToDto())
            .ToListAsync(cancellationToken);

        return new Response(dto);
    }
}