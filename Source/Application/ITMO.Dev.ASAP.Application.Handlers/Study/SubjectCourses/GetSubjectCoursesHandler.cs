using ITMO.Dev.ASAP.Application.Dto.SubjectCourses;
using ITMO.Dev.ASAP.Application.Handlers.Extensions;
using ITMO.Dev.ASAP.Core.Study;
using ITMO.Dev.ASAP.DataAccess.Abstractions;
using ITMO.Dev.ASAP.Github.Presentation.Contracts.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static ITMO.Dev.ASAP.Application.Contracts.Study.SubjectCourses.Queries.GetSubjectCourses;

namespace ITMO.Dev.ASAP.Application.Handlers.Study.SubjectCourses;

internal class GetSubjectCoursesHandler : IRequestHandler<Query, Response>
{
    private readonly IDatabaseContext _context;
    private readonly IGithubSubjectCourseService _githubSubjectCourseService;

    public GetSubjectCoursesHandler(IDatabaseContext context, IGithubSubjectCourseService githubSubjectCourseService)
    {
        _context = context;
        _githubSubjectCourseService = githubSubjectCourseService;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        List<SubjectCourse> subjectCourses = await _context
            .SubjectCourses
            .ToListAsync(cancellationToken);

        IReadOnlyCollection<SubjectCourseDto> dto = await _githubSubjectCourseService
            .MapToSubjectCourseDtoAsync(subjectCourses, cancellationToken);

        return new Response(dto);
    }
}