using ITMO.Dev.ASAP.Application.Contracts.Study.SubjectCourseGroups.Commands;
using ITMO.Dev.ASAP.Application.Dto.SubjectCourses;
using ITMO.Dev.ASAP.Authorization;
using ITMO.Dev.ASAP.WebApi.Abstractions.Models.SubjectCourseGroups;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ITMO.Dev.ASAP.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SubjectCourseGroupController : ControllerBase
{
    private const string Scope = "SubjectCourseGroups";

    private readonly IMediator _mediator;

    public SubjectCourseGroupController(IMediator mediator)
    {
        _mediator = mediator;
    }

    public CancellationToken CancellationToken => HttpContext.RequestAborted;

    [HttpPost]
    [AuthorizeFeature(Scope, nameof(Create))]
    public async Task<ActionResult<SubjectCourseGroupDto>> Create(
        CreateSubjectCourseGroupRequest request)
    {
        (Guid subjectCourseId, Guid groupId) = request;

        var command = new CreateSubjectCourseGroup.Command(subjectCourseId, groupId);
        CreateSubjectCourseGroup.Response result = await _mediator.Send(command, CancellationToken);

        return Ok(result);
    }

    [HttpPost("bulk")]
    [AuthorizeFeature(Scope, nameof(BulkCreate))]
    public async Task<ActionResult<IReadOnlyCollection<SubjectCourseGroupDto>>> BulkCreate(
        BulkCreateSubjectCourseGroupsRequest request)
    {
        var command = new BulkCreateSubjectCourseGroups.Command(request.SubjectCourseId, request.GroupIds);
        BulkCreateSubjectCourseGroups.Response response = await _mediator.Send(command, CancellationToken);

        return Ok(response.Groups);
    }

    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [AuthorizeFeature(Scope, nameof(Delete))]
    public async Task<IActionResult> Delete(DeleteSubjectCourseGroupRequest request)
    {
        (Guid subjectCourseId, Guid groupId) = request;

        var command = new DeleteSubjectCourseGroup.Command(subjectCourseId, groupId);
        await _mediator.Send(command, CancellationToken);

        return NoContent();
    }
}