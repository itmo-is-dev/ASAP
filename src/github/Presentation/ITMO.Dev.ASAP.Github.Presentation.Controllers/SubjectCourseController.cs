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
    private readonly IMediator _mediator;

    public SubjectCourseController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("github")]
    public async Task<ActionResult<GithubSubjectCourseDto>> CreateGithubSubjectCourseAsync(
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
    public async Task<ActionResult> UpdateMentorsTeamNameAsync(
        Guid subjectCourseId,
        [FromBody] UpdateMentorsTeamNameRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateSubjectCourseMentorTeam.Command(subjectCourseId, request.TeamName);
        await _mediator.Send(command, cancellationToken);

        return Ok();
    }
}