using ITMO.Dev.ASAP.Application.Dto.SubjectCourses;
using ITMO.Dev.ASAP.Application.Handlers.Extensions;
using ITMO.Dev.ASAP.Core.Study;
using ITMO.Dev.ASAP.DataAccess.Abstractions;
using ITMO.Dev.ASAP.Github.Presentation.Contracts.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static ITMO.Dev.ASAP.Application.Contracts.Study.SubjectCourses.Queries.GetSubjectCoursesBySubjectId;

namespace ITMO.Dev.ASAP.Application.Handlers.Study.SubjectCourses;

internal class GetSubjectCoursesBySubjectIdHandler : IRequestHandler<Query, Response>
{
    private readonly IDatabaseContext _context;
    private readonly IGithubSubjectCourseService _githubSubjectCourseService;

    public GetSubjectCoursesBySubjectIdHandler(
        IDatabaseContext context,
        IGithubSubjectCourseService githubSubjectCourseService)
    {
        _context = context;
        _githubSubjectCourseService = githubSubjectCourseService;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        List<SubjectCourse> courses = await _context.SubjectCourses
            .Where(x => x.Subject.Id.Equals(request.SubjectId))
            .ToListAsync(cancellationToken);

        IReadOnlyCollection<SubjectCourseDto> dto = await _githubSubjectCourseService
            .MapToSubjectCourseDtoAsync(courses, cancellationToken);

        return new Response(dto);
    }
}