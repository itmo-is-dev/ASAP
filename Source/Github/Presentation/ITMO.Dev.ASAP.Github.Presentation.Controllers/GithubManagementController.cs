using ITMO.Dev.ASAP.Github.Application.Contracts.SubjectCourses.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ITMO.Dev.ASAP.Github.Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Admin")]
public class GithubManagementController : ControllerBase
{
    private readonly IMediator _mediator;

    public GithubManagementController(IMediator mediator)
    {
        _mediator = mediator;
    }

    public CancellationToken CancellationToken => HttpContext.RequestAborted;

    [HttpPost("force-update")]
    public async Task<IActionResult> ForceOrganizationUpdateAsync([FromQuery] Guid? subjectCourseId)
    {
        IRequest request = subjectCourseId is null
            ? new UpdateSubjectCourseOrganizations.Command()
            : new UpdateSubjectCourseOrganization.Command(subjectCourseId.Value);

        await _mediator.Send(request, CancellationToken);

        return Ok();
    }

    [HttpPost("force-mentor-sync")]
    public async Task<ActionResult> ForceMentorsSync(string organizationName)
    {
        var command = new SyncGithubMentors.Command(organizationName);

        await _mediator.Send(command, CancellationToken);

        return Ok();
    }
}