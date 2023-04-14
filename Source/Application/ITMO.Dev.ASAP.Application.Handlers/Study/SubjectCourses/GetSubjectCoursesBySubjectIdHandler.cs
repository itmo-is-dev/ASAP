using ITMO.Dev.ASAP.Application.Abstractions.Identity;
using ITMO.Dev.ASAP.Application.Common.Exceptions;
using ITMO.Dev.ASAP.Application.Dto.SubjectCourses;
using ITMO.Dev.ASAP.Application.Extensions;
using ITMO.Dev.ASAP.Core.Study;
using ITMO.Dev.ASAP.DataAccess.Abstractions;
using ITMO.Dev.ASAP.DataAccess.Abstractions.Extensions;
using ITMO.Dev.ASAP.Github.Presentation.Contracts.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static ITMO.Dev.ASAP.Application.Contracts.Study.SubjectCourses.Queries.GetSubjectCoursesBySubjectId;

namespace ITMO.Dev.ASAP.Application.Handlers.Study.SubjectCourses;

internal class GetSubjectCoursesBySubjectIdHandler : IRequestHandler<Query, Response>
{
    private readonly IDatabaseContext _context;
    private readonly IGithubSubjectCourseService _githubSubjectCourseService;
    private readonly ICurrentUser _currentUser;

    public GetSubjectCoursesBySubjectIdHandler(
        IDatabaseContext context,
        IGithubSubjectCourseService githubSubjectCourseService,
        ICurrentUser currentUser)
    {
        _context = context;
        _githubSubjectCourseService = githubSubjectCourseService;
        _currentUser = currentUser;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        Subject subject = await _context.Subjects
            .Include(x => x.Courses)
            .GetByIdAsync(request.SubjectId, cancellationToken);

        if (_currentUser.HasAccessToSubject(subject) is false)
            throw UserHasNotAccessException.AccessViolation(_currentUser.Id);

        var availableSubjectCourses = subject.Courses
            .Where(_currentUser.HasAccessToSubjectCourse)
            .ToList();

        IReadOnlyCollection<SubjectCourseDto> dto = await _githubSubjectCourseService
            .MapToSubjectCourseDtoAsync(availableSubjectCourses, cancellationToken);

        return new Response(dto);
    }
}