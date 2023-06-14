using ITMO.Dev.ASAP.Application.Contracts.Students.Queries;
using ITMO.Dev.ASAP.Application.Contracts.Study.StudyGroups.Commands;
using ITMO.Dev.ASAP.Application.Contracts.Study.StudyGroups.Queries;
using ITMO.Dev.ASAP.Application.Dto.Querying;
using ITMO.Dev.ASAP.Application.Dto.Study;
using ITMO.Dev.ASAP.Application.Dto.Users;
using ITMO.Dev.ASAP.Authorization;
using ITMO.Dev.ASAP.WebApi.Abstractions.Models.StudyGroups;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ITMO.Dev.ASAP.Controllers;

[Route("api/[controller]")]
[ApiController]
public class StudyGroupController : ControllerBase
{
    private const string Scope = "StudyGroup";

    private readonly IMediator _mediator;

    public StudyGroupController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [AuthorizeFeature(Scope, nameof(Create))]
    public async Task<ActionResult<StudyGroupDto>> Create(CreateStudyGroupRequest request)
    {
        var command = new CreateStudyGroup.Command(request.Name);
        CreateStudyGroup.Response response = await _mediator.Send(command);

        return Ok(response.Group);
    }

    [HttpGet("bulk")]
    [AuthorizeFeature(Scope, nameof(BulkGetByIds))]
    public async Task<ActionResult<IReadOnlyCollection<StudyGroupDto>>> BulkGetByIds(
        [FromQuery] IReadOnlyCollection<Guid> ids)
    {
        var query = new BulkGetStudyGroups.Query(ids);
        BulkGetStudyGroups.Response response = await _mediator.Send(query);

        return Ok(response.Groups);
    }

    [HttpPut("{id:guid}")]
    [AuthorizeFeature(Scope, nameof(Update))]
    public async Task<ActionResult<StudyGroupDto>> Update(Guid id, UpdateStudyGroupRequest request)
    {
        var command = new UpdateStudyGroup.Command(id, request.Name);
        UpdateStudyGroup.Response response = await _mediator.Send(command);

        return Ok(response.Group);
    }

    [HttpGet("{id:guid}/students")]
    [AuthorizeFeature(Scope, nameof(GetStudents))]
    public async Task<ActionResult<IReadOnlyCollection<StudentDto>>> GetStudents(Guid id)
    {
        var query = new GetStudentsByGroupId.Query(id);
        GetStudentsByGroupId.Response response = await _mediator.Send(query);

        return Ok(response.Students);
    }

    [HttpPost("query")]
    [AuthorizeFeature(Scope, nameof(Query))]
    public async Task<ActionResult<IReadOnlyCollection<StudyGroupDto>>> Query(
        QueryConfiguration<GroupQueryParameter> configuration)
    {
        var query = new FindGroupsByQuery.Query(configuration);
        FindGroupsByQuery.Response response = await _mediator.Send(query);

        return Ok(response.Groups);
    }
}