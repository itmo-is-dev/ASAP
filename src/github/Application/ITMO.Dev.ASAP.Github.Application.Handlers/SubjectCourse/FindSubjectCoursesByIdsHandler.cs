using ITMO.Dev.ASAP.Github.Application.DataAccess;
using ITMO.Dev.ASAP.Github.Application.Dto.SubjectCourses;
using ITMO.Dev.ASAP.Github.Application.Mapping;
using ITMO.Dev.ASAP.Github.Domain.SubjectCourses;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static ITMO.Dev.ASAP.Github.Application.Contracts.SubjectCourses.Queries.FindSubjectCoursesByIds;

namespace ITMO.Dev.ASAP.Github.Application.Handlers.SubjectCourse;

internal class FindSubjectCoursesByIdsHandler : IRequestHandler<Query, Response>
{
    private readonly IDatabaseContext _context;

    public FindSubjectCoursesByIdsHandler(IDatabaseContext context)
    {
        _context = context;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        List<GithubSubjectCourse> subjectCourses = await _context.SubjectCourses
            .Where(x => request.Ids.Contains(x.Id))
            .ToListAsync(cancellationToken);

        GithubSubjectCourseDto[] dto = subjectCourses.Select(x => x.ToDto()).ToArray();

        return new Response(dto);
    }
}