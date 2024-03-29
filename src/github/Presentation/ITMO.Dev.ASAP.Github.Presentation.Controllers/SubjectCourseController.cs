using ITMO.Dev.ASAP.Authorization;
using ITMO.Dev.ASAP.Github.Application.Contracts.SubjectCourses.Commands;
using ITMO.Dev.ASAP.Github.Application.Dto.SubjectCourses;
using ITMO.Dev.ASAP.WebApi.Abstractions.Models.Github;
using ITMO.Dev.ASAP.WebApi.Abstractions.Models.SubjectCourses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ITMO.Dev.ASAP.Github.Presentation.Controllers;

[Route("api/[controller]")]
public class SubjectCourseController : ControllerBase
{
    private const string Scope = "SubjectCourse";

    private readonly IMediator _mediator;

    public SubjectCourseController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("github")]
    [AuthorizeFeature(Scope, nameof(GithubCreate))]
    public async Task<ActionResult<GithubSubjectCourseDto>> GithubCreate(
        [FromBody] CreateGithubSubjectCourseRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateSubjectCourse.Command(
            request.SubjectId,
            request.Name,
            request.WorkflowType,
            request.GithubOrganizationName,
            request.TemplateRepositoryName,
            request.MentorTeamName);

        CreateSubjectCourse.Response response = await _mediator.Send(command, cancellationToken);

        return Ok(response.SubjectCourse);
    }

    [HttpPut("{subjectCourseId:guid}/github/mentor-team")]
    [AuthorizeFeature(Scope, nameof(GithubUpdateMentorsTeam))]
    public async Task<ActionResult> GithubUpdateMentorsTeam(
        Guid subjectCourseId,
        [FromBody] UpdateMentorsTeamNameRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateSubjectCourseMentorTeam.Command(subjectCourseId, request.TeamName);
        await _mediator.Send(command, cancellationToken);

        return Ok();
    }
}