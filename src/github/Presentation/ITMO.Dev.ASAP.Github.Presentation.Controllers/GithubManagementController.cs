using ITMO.Dev.ASAP.Authorization;
using ITMO.Dev.ASAP.Github.Application.Contracts.SubjectCourses.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ITMO.Dev.ASAP.Github.Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
public class GithubManagementController : ControllerBase
{
    private const string Scope = "Github";

    private readonly IMediator _mediator;

    public GithubManagementController(IMediator mediator)
    {
        _mediator = mediator;
    }

    public CancellationToken CancellationToken => HttpContext.RequestAborted;

    [HttpPost("force-update")]
    [AuthorizeFeature(Scope, nameof(ForceOrganizationUpdate))]
    public async Task<IActionResult> ForceOrganizationUpdate([FromQuery] Guid? subjectCourseId)
    {
        IRequest request = subjectCourseId is null
            ? new UpdateSubjectCourseOrganizations.Command()
            : new UpdateSubjectCourseOrganization.Command(subjectCourseId.Value);

        await _mediator.Send(request, CancellationToken);

        return Ok();
    }

    [HttpPost("force-mentor-sync")]
    [AuthorizeFeature(Scope, nameof(ForceMentorSync))]
    public async Task<ActionResult> ForceMentorSync(string organizationName)
    {
        var command = new SyncGithubMentors.Command(organizationName);

        await _mediator.Send(command, CancellationToken);

        return Ok();
    }
}