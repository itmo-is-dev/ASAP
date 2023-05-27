using ITMO.Dev.ASAP.Application.Contracts.Study.SubjectCourses.Queries;
using ITMO.Dev.ASAP.Application.Contracts.Study.Subjects.Commands;
using ITMO.Dev.ASAP.Application.Contracts.Study.Subjects.Queries;
using ITMO.Dev.ASAP.Application.Dto.Study;
using ITMO.Dev.ASAP.Application.Dto.SubjectCourses;
using ITMO.Dev.ASAP.Authorization;
using ITMO.Dev.ASAP.WebApi.Abstractions.Models.Subjects;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ITMO.Dev.ASAP.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SubjectController : ControllerBase
{
    private const string Scope = "Subjects";

    private readonly IMediator _mediator;

    public SubjectController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [AuthorizeFeature(Scope, nameof(Create))]
    public async Task<ActionResult<SubjectDto>> Create(CreateSubjectRequest request)
    {
        var command = new CreateSubject.Command(request.Name);
        CreateSubject.Response response = await _mediator.Send(command);

        return Ok(response.Subject);
    }

    [HttpGet]
    [AuthorizeFeature(Scope, nameof(GetAll))]
    public async Task<ActionResult<IReadOnlyCollection<SubjectDto>>> GetAll()
    {
        var query = new GetSubjects.Query();
        GetSubjects.Response response = await _mediator.Send(query);

        return Ok(response.Subjects);
    }

    [HttpGet("{id:guid}")]
    [AuthorizeFeature(Scope, nameof(GetById))]
    public async Task<ActionResult<SubjectDto>> GetById(Guid id)
    {
        var query = new GetSubjectById.Query(id);
        GetSubjectById.Response response = await _mediator.Send(query);

        return Ok(response.Subject);
    }

    [HttpPut("{id:guid}")]
    [AuthorizeFeature(Scope, nameof(Update))]
    public async Task<ActionResult<SubjectDto>> Update(Guid id, UpdateSubjectRequest request)
    {
        var command = new UpdateSubject.Command(id, request.Name);
        UpdateSubject.Response response = await _mediator.Send(command);

        return Ok(response.Subject);
    }

    [HttpGet("{id:guid}/courses")]
    [AuthorizeFeature(Scope, nameof(GetSubjectCourses))]
    public async Task<ActionResult<IReadOnlyCollection<SubjectCourseDto>>> GetSubjectCourses(Guid id)
    {
        var request = new GetSubjectCoursesBySubjectId.Query(id);
        GetSubjectCoursesBySubjectId.Response
            response = await _mediator.Send(request, HttpContext.RequestAborted);

        return Ok(response.Courses);
    }
}