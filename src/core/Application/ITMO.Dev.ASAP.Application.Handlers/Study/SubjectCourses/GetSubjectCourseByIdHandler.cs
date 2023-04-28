using ITMO.Dev.ASAP.Application.Abstractions.Identity;
using ITMO.Dev.ASAP.Application.Common.Exceptions;
using ITMO.Dev.ASAP.Application.Dto.SubjectCourseAssociations;
using ITMO.Dev.ASAP.DataAccess.Abstractions;
using ITMO.Dev.ASAP.DataAccess.Abstractions.Extensions;
using ITMO.Dev.ASAP.Domain.Study;
using ITMO.Dev.ASAP.Github.Application.Dto.SubjectCourses;
using ITMO.Dev.ASAP.Github.Presentation.Contracts.Services;
using ITMO.Dev.ASAP.Google.Application.Dto.SubjectCourses;
using ITMO.Dev.ASAP.Google.Presentation.Contracts.Services;
using ITMO.Dev.ASAP.Mapping.Mappings;
using MediatR;
using static ITMO.Dev.ASAP.Application.Contracts.Study.SubjectCourses.Queries.GetSubjectCourseById;

namespace ITMO.Dev.ASAP.Application.Handlers.Study.SubjectCourses;

internal class GetSubjectCourseByIdHandler : IRequestHandler<Query, Response>
{
    private readonly IDatabaseContext _context;
    private readonly ICurrentUser _currentUser;
    private readonly IGithubSubjectCourseService _githubSubjectCourseService;
    private readonly IGoogleSubjectCourseService _googleSubjectCourseService;

    public GetSubjectCourseByIdHandler(
        IDatabaseContext context,
        ICurrentUser currentUser,
        IGithubSubjectCourseService githubSubjectCourseService,
        IGoogleSubjectCourseService googleSubjectCourseService)
    {
        _context = context;
        _currentUser = currentUser;
        _githubSubjectCourseService = githubSubjectCourseService;
        _googleSubjectCourseService = googleSubjectCourseService;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        SubjectCourse subjectCourse = await _context.SubjectCourses
            .GetByIdAsync(request.Id, cancellationToken);

        if (_currentUser.HasAccessToSubjectCourse(subjectCourse) is false)
            throw UserHasNotAccessException.AccessViolation(_currentUser.Id);

        IReadOnlyCollection<GithubSubjectCourseDto> githubSubjectCourses = await _githubSubjectCourseService
            .FindByIdsAsync(new[] { subjectCourse.Id }, cancellationToken);

        IEnumerable<GoogleSubjectCourseDto> googleSubjectCourses = await _googleSubjectCourseService
            .FindByIdsAsync(new[] { subjectCourse.Id }, cancellationToken);

        IEnumerable<SubjectCourseAssociationDto> githubAssociations = githubSubjectCourses
            .Select(x => x.ToAssociationDto());

        IEnumerable<SubjectCourseAssociationDto> googleAssociations = googleSubjectCourses
            .Select(x => x.ToAssociationDto());

        IEnumerable<SubjectCourseAssociationDto> associations = githubAssociations.Concat(googleAssociations);

        return new Response(subjectCourse.ToDto(associations));
    }
}