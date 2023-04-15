using ITMO.Dev.ASAP.Application.Dto.SubjectCourses;
using ITMO.Dev.ASAP.Github.Application.DataAccess;
using ITMO.Dev.ASAP.Github.Application.Dto.SubjectCourses;
using ITMO.Dev.ASAP.Github.Application.Mapping;
using ITMO.Dev.ASAP.Github.Domain.SubjectCourses;
using ITMO.Dev.ASAP.Presentation.Contracts.Services;
using MediatR;
using static ITMO.Dev.ASAP.Github.Application.Contracts.SubjectCourses.Commands.CreateSubjectCourse;

namespace ITMO.Dev.ASAP.Github.Application.Handlers.SubjectCourse;

internal class CreateSubjectCourseHandler : IRequestHandler<Command, Response>
{
    private readonly IDatabaseContext _context;
    private readonly IAsapSubjectCourseService _asapSubjectCourseService;

    public CreateSubjectCourseHandler(IDatabaseContext context, IAsapSubjectCourseService asapSubjectCourseService)
    {
        _context = context;
        _asapSubjectCourseService = asapSubjectCourseService;
    }

    public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
    {
        SubjectCourseDto subjectCourse = await _asapSubjectCourseService.CreateSubjectCourseAsync(
            request.SubjectId,
            request.Name,
            request.WorkflowType,
            cancellationToken);

        var githubSubjectCourse = new GithubSubjectCourse(
            subjectCourse.Id,
            request.GithubOrganizationName,
            request.TemplateRepositoryName,
            request.MentorTeamName);

        _context.SubjectCourses.Add(githubSubjectCourse);
        await _context.SaveChangesAsync(default);

        GithubSubjectCourseDto dto = githubSubjectCourse.ToDto();

        return new Response(dto);
    }
}