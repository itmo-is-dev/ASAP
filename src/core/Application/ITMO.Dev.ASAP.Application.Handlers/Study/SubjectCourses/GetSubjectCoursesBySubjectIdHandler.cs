using ITMO.Dev.ASAP.Application.Abstractions.Identity;
using ITMO.Dev.ASAP.Application.Common.Exceptions;
using ITMO.Dev.ASAP.Application.DataAccess;
using ITMO.Dev.ASAP.Application.DataAccess.Queries;
using ITMO.Dev.ASAP.Application.Dto.SubjectCourseAssociations;
using ITMO.Dev.ASAP.Application.Dto.SubjectCourses;
using ITMO.Dev.ASAP.Domain.Study;
using ITMO.Dev.ASAP.Github.Application.Dto.SubjectCourses;
using ITMO.Dev.ASAP.Github.Presentation.Contracts.Services;
using ITMO.Dev.ASAP.Google.Application.Dto.SubjectCourses;
using ITMO.Dev.ASAP.Google.Presentation.Contracts.Services;
using ITMO.Dev.ASAP.Mapping.Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static ITMO.Dev.ASAP.Application.Contracts.Study.SubjectCourses.Queries.GetSubjectCoursesBySubjectId;

namespace ITMO.Dev.ASAP.Application.Handlers.Study.SubjectCourses;

internal class GetSubjectCoursesBySubjectIdHandler : IRequestHandler<Query, Response>
{
    private readonly IDatabaseContext _context;
    private readonly IGithubSubjectCourseService _githubSubjectCourseService;
    private readonly ICurrentUser _currentUser;
    private readonly IGoogleSubjectCourseService _googleSubjectCourseService;

    public GetSubjectCoursesBySubjectIdHandler(
        IDatabaseContext context,
        IGithubSubjectCourseService githubSubjectCourseService,
        ICurrentUser currentUser,
        IGoogleSubjectCourseService googleSubjectCourseService)
    {
        _context = context;
        _githubSubjectCourseService = githubSubjectCourseService;
        _currentUser = currentUser;
        _googleSubjectCourseService = googleSubjectCourseService;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        var query = SubjectQuery.Build(x => _currentUser.FilterAvailableSubjects(x).WithId(request.SubjectId));

        Subject? subject = await _context.Subjects
            .QueryAsync(query, cancellationToken)
            .SingleOrDefaultAsync(cancellationToken);

        if (subject is null)
            throw UserHasNotAccessException.AccessViolation(_currentUser.Id);

        List<SubjectCourse> courses = await _context.SubjectCourses
            .Where(x => x.SubjectId.Equals(subject.Id))
            .ToListAsync(cancellationToken);

        var availableSubjectCourses = courses
            .Where(_currentUser.HasAccessToSubjectCourse)
            .ToList();

        IEnumerable<Guid> ids = availableSubjectCourses.Select(x => x.Id);

        IReadOnlyCollection<GithubSubjectCourseDto> githubSubjectCourses = await _githubSubjectCourseService
            .FindByIdsAsync(ids, cancellationToken);

        IEnumerable<GoogleSubjectCourseDto> googleSubjectCourses = await _googleSubjectCourseService
            .FindByIdsAsync(ids, cancellationToken);

        IEnumerable<SubjectCourseAssociationDto> githubAssociations = githubSubjectCourses
            .Select(x => x.ToAssociationDto());

        IEnumerable<SubjectCourseAssociationDto> googleAssociations = googleSubjectCourses
            .Select(x => x.ToAssociationDto());

        IEnumerable<SubjectCourseAssociationDto> associations = githubAssociations.Concat(googleAssociations);

        SubjectCourseDto[] dto = availableSubjectCourses
            .GroupJoin(
                associations,
                x => x.Id,
                x => x.SubjectCourseId,
                (c, a) => c.ToDto(a))
            .ToArray();

        return new Response(dto);
    }
}