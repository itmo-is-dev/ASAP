using ITMO.Dev.ASAP.Application.Abstractions.SubjectCourses;
using ITMO.Dev.ASAP.Application.Contracts.Students.Queries;
using ITMO.Dev.ASAP.Application.Contracts.Study.Assignments.Queries;
using ITMO.Dev.ASAP.Application.Contracts.Study.Queues.Queries;
using ITMO.Dev.ASAP.Application.Contracts.Study.SubjectCourseGroups.Queries;
using ITMO.Dev.ASAP.Application.Contracts.Study.SubjectCourses.Commands;
using ITMO.Dev.ASAP.Application.Contracts.Study.SubjectCourses.Queries;
using ITMO.Dev.ASAP.Application.Dto.Study;
using ITMO.Dev.ASAP.Application.Dto.SubjectCourses;
using ITMO.Dev.ASAP.Application.Dto.Tables;
using ITMO.Dev.ASAP.Application.Dto.Users;
using ITMO.Dev.ASAP.Authorization;
using ITMO.Dev.ASAP.WebApi.Abstractions.Models.SubjectCourses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ITMO.Dev.ASAP.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SubjectCourseController : ControllerBase
{
    private const string Scope = "SubjectCourse";

    private readonly IMediator _mediator;
    private readonly ISubjectCourseUpdateService _subjectCourseUpdateService;

    public SubjectCourseController(IMediator mediator, ISubjectCourseUpdateService subjectCourseUpdateService)
    {
        _mediator = mediator;
        _subjectCourseUpdateService = subjectCourseUpdateService;
    }

    public CancellationToken CancellationToken => HttpContext.RequestAborted;

    [HttpGet("{id:guid}")]
    [AuthorizeFeature(Scope, nameof(GetById))]
    public async Task<ActionResult<SubjectCourseDto>> GetById(Guid id)
    {
        var query = new GetSubjectCourseById.Query(id);
        GetSubjectCourseById.Response response = await _mediator.Send(query, CancellationToken);

        return Ok(response.SubjectCourse);
    }

    [HttpPut("{id:guid}")]
    [AuthorizeFeature(Scope, nameof(Update))]
    public async Task<ActionResult<SubjectCourseDto>> Update(Guid id, UpdateSubjectCourseRequest request)
    {
        var command = new UpdateSubjectCourse.Command(id, request.Name);
        UpdateSubjectCourse.Response response = await _mediator.Send(command, CancellationToken);

        return Ok(response.SubjectCourse);
    }

    [HttpGet("{id:guid}/students")]
    [AuthorizeFeature(Scope, nameof(GetStudents))]
    public async Task<ActionResult<IReadOnlyCollection<StudentDto>>> GetStudents(Guid id)
    {
        var query = new GetStudentsBySubjectCourseId.Query(id);
        GetStudentsBySubjectCourseId.Response response = await _mediator.Send(query, CancellationToken);

        return Ok(response.Students);
    }

    [HttpGet("{id:guid}/assignments")]
    [AuthorizeFeature(Scope, nameof(GetAssignments))]
    public async Task<ActionResult<IReadOnlyCollection<AssignmentDto>>> GetAssignments(Guid id)
    {
        var query = new GetAssignmentsBySubjectCourse.Query(id);
        GetAssignmentsBySubjectCourse.Response response = await _mediator.Send(query, CancellationToken);

        return Ok(response.Assignments);
    }

    [HttpGet("{id:guid}/groups")]
    [AuthorizeFeature(Scope, nameof(GetGroups))]
    public async Task<ActionResult<IReadOnlyCollection<SubjectCourseGroupDto>>> GetGroups(Guid id)
    {
        var query = new GetSubjectCourseGroupsBySubjectCourseId.Query(id);
        GetSubjectCourseGroupsBySubjectCourseId.Response result = await _mediator.Send(query, CancellationToken);

        return Ok(result.Groups);
    }

    [HttpGet("{subjectCourseId:guid}/groups/{studyGroupId:guid}/queue")]
    [AuthorizeFeature(Scope, nameof(GetStudyGroupQueue))]
    public async Task<ActionResult<SubmissionsQueueDto>> GetStudyGroupQueue(
        Guid subjectCourseId,
        Guid studyGroupId,
        CancellationToken cancellationToken)
    {
        var queue = new GetSubmissionsQueue.Query(subjectCourseId, studyGroupId);
        GetSubmissionsQueue.Response response = await _mediator.Send(queue, cancellationToken);

        return Ok(response.SubmissionsQueue);
    }

    [HttpPost("{id:guid}/deadline/fraction")]
    [AuthorizeFeature(Scope, nameof(AddDeadline))]
    public async Task<ActionResult> AddDeadline(Guid id, AddFractionPolicyRequest request)
    {
        (TimeSpan spanBeforeActivation, double fraction) = request;

        var command = new AddFractionDeadlinePolicy.Command(id, spanBeforeActivation, fraction);
        await _mediator.Send(command, CancellationToken);

        return Ok();
    }

    [HttpPost("{id:guid}/points/force-sync")]
    [AuthorizeFeature(Scope, nameof(ForceSyncPoints))]
    public IActionResult ForceSyncPoints(Guid id)
    {
        _subjectCourseUpdateService.UpdatePoints(id);
        return Ok();
    }
}