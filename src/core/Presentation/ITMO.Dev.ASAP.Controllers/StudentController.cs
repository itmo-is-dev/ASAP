using ITMO.Dev.ASAP.Application.Contracts.Students.Commands;
using ITMO.Dev.ASAP.Application.Contracts.Users.Commands;
using ITMO.Dev.ASAP.Application.Contracts.Users.Queries;
using ITMO.Dev.ASAP.Application.Dto.Querying;
using ITMO.Dev.ASAP.Application.Dto.Users;
using ITMO.Dev.ASAP.Authorization;
using ITMO.Dev.ASAP.WebApi.Abstractions.Models.Students;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ITMO.Dev.ASAP.Controllers;

[Route("api/[controller]")]
[ApiController]
public class StudentController : ControllerBase
{
    private const string Scope = "Student";

    private readonly IMediator _mediator;

    public StudentController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [AuthorizeFeature(Scope, nameof(Create))]
    public async Task<ActionResult<StudentDto>> Create(CreateStudentRequest request)
    {
        (string? firstName, string? middleName, string? lastName, Guid groupId) = request;

        var command = new CreateStudent.Command(
            firstName ?? string.Empty,
            middleName ?? string.Empty,
            lastName ?? string.Empty,
            groupId);

        CreateStudent.Response response = await _mediator.Send(command);
        return Ok(response.Student);
    }

    [HttpPut("{id:guid}/dismiss")]
    [AuthorizeFeature(Scope, nameof(DismissFromGroup))]
    public async Task<ActionResult> DismissFromGroup(Guid id)
    {
        var command = new DismissStudentFromGroup.Command(id);
        await _mediator.Send(command);

        return Ok();
    }

    [HttpPut("{id:guid}/group")]
    [AuthorizeFeature(Scope, nameof(Transfer))]
    public async Task<ActionResult<StudentDto>> Transfer(Guid id, TransferStudentRequest request)
    {
        var command = new TransferStudent.Command(id, request.NewGroupId);
        TransferStudent.Response response = await _mediator.Send(command);

        return Ok(response.Student);
    }

    [HttpPost("query")]
    [AuthorizeFeature(Scope, nameof(Query))]
    public async Task<ActionResult<IReadOnlyCollection<StudentDto>>> Query(
        QueryConfiguration<StudentQueryParameter> configuration)
    {
        var query = new FindStudentsByQuery.Query(configuration);
        FindStudentsByQuery.Response response = await _mediator.Send(query);

        return Ok(response.Students);
    }
}