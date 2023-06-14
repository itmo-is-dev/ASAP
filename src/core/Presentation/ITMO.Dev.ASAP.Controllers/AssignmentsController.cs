using ITMO.Dev.ASAP.Application.Contracts.Study.Assignments.Commands;
using ITMO.Dev.ASAP.Application.Contracts.Study.GroupAssignments.Commands;
using ITMO.Dev.ASAP.Application.Contracts.Study.GroupAssignments.Queries;
using ITMO.Dev.ASAP.Application.Dto.Study;
using ITMO.Dev.ASAP.Authorization;
using ITMO.Dev.ASAP.WebApi.Abstractions.Models;
using ITMO.Dev.ASAP.WebApi.Abstractions.Models.GroupAssignments;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ITMO.Dev.ASAP.Controllers;

[Route("/api/[controller]")]
[ApiController]
public class AssignmentsController : ControllerBase
{
    private const string Scope = "Assignments";

    private readonly IMediator _mediator;

    public AssignmentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [AuthorizeFeature(Scope, nameof(Create))]
    public async Task<ActionResult<AssignmentDto>> Create(CreateAssignmentRequest request)
    {
        var command = new CreateAssignment.Command(
            request.SubjectCourseId,
            request.Title,
            request.ShortName,
            request.Order,
            request.MinPoints,
            request.MaxPoints);

        CreateAssignment.Response result = await _mediator.Send(command);
        return Ok(result.Assignment);
    }

    [HttpPatch("{id:guid}")]
    [AuthorizeFeature(Scope, nameof(UpdatePoints))]
    public async Task<ActionResult<AssignmentDto>> UpdatePoints(Guid id, double minPoints, double maxPoints)
    {
        var command = new UpdateAssignmentPoints.Command(id, minPoints, maxPoints);

        UpdateAssignmentPoints.Response response = await _mediator.Send(command);

        return Ok(response.Assignment);
    }

    [HttpGet("{assignmentId:guid}/groups")]
    [AuthorizeFeature(Scope, nameof(GetGroups))]
    public async Task<ActionResult<IReadOnlyCollection<GroupAssignmentDto>>> GetGroups(Guid assignmentId)
    {
        var query = new GetGroupAssignments.Query(assignmentId);
        GetGroupAssignments.Response response = await _mediator.Send(query);

        return Ok(response.GroupAssignments);
    }

    [HttpPut("{assignmentId:guid}/groups/{groupId:guid}")]
    [AuthorizeFeature(Scope, nameof(UpdateGroupAssignment))]
    public async Task<ActionResult<GroupAssignmentDto>> UpdateGroupAssignment(
        Guid assignmentId,
        Guid groupId,
        UpdateGroupAssignmentRequest request)
    {
        var deadline = DateOnly.FromDateTime(request.Deadline);
        var command = new UpdateGroupAssignmentDeadline.Command(groupId, assignmentId, deadline);
        UpdateGroupAssignmentDeadline.Response response = await _mediator.Send(command);

        return Ok(response.GroupAssignment);
    }
}