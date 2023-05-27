using ITMO.Dev.ASAP.Application.Contracts.Students.Commands;
using ITMO.Dev.ASAP.Application.Contracts.Users.Commands;
using ITMO.Dev.ASAP.Application.Contracts.Users.Queries;
using ITMO.Dev.ASAP.Application.Dto.Querying;
using ITMO.Dev.ASAP.Application.Dto.Users;
using ITMO.Dev.ASAP.Authorization;
using ITMO.Dev.ASAP.WebApi.Abstractions.Models.Users;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ITMO.Dev.ASAP.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private const string Scope = "Users";

    private readonly IMediator _mediator;

    public UserController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(200)]
    [ProducesResponseType(204)]
    [AuthorizeFeature(Scope, nameof(FindByUniversityId))]
    public async Task<ActionResult<UserDto?>> FindByUniversityId(int? universityId, CancellationToken cancellationToken)
    {
        UserDto? user;

        if (universityId is not null)
        {
            user = await FindUserByUniversityIdAsync(universityId.Value, cancellationToken);
        }
        else
        {
            user = await FindCurrentUserAsync(cancellationToken);
        }

        return user is not null ? Ok(user) : NoContent();
    }

    [HttpPost("{userId:guid}/universityId/update")]
    [AuthorizeFeature(Scope, nameof(UpdateUniversityId))]
    public async Task<IActionResult> UpdateUniversityId(Guid userId, int universityId)
    {
        var command = new UpdateUserUniversityId.Command(userId, universityId);
        await _mediator.Send(command);

        return Ok();
    }

    [HttpPost("{userId:guid}/change-name")]
    [AuthorizeFeature(Scope, nameof(UpdateName))]
    public async Task<ActionResult> UpdateName(Guid userId, string firstName, string middleName, string lastName)
    {
        await _mediator.Send(new UpdateUserName.Command(userId, firstName, middleName, lastName));
        return Ok();
    }

    [ProducesResponseType(200)]
    [HttpPost("identity-info")]
    [AuthorizeFeature(Scope, nameof(QueryIdentityInfo))]
    public async Task<ActionResult<GetUserIdentityInfosResponse>> QueryIdentityInfo(
        [FromBody] QueryConfiguration<UserQueryParameter> queryConfiguration,
        int? page,
        CancellationToken cancellationToken)
    {
        var query = new GetUserIdentityInfos.Query(queryConfiguration, page ?? 0);
        GetUserIdentityInfos.Response response = await _mediator.Send(query, cancellationToken);

        return Ok(new GetUserIdentityInfosResponse(response.Users, response.PageCount));
    }

    private async Task<UserDto?> FindCurrentUserAsync(CancellationToken cancellationToken)
    {
        var query = new FindCurrentUser.Query();
        FindCurrentUser.Response response = await _mediator.Send(query, cancellationToken);

        return response.User;
    }

    private async Task<UserDto?> FindUserByUniversityIdAsync(int universityId, CancellationToken cancellationToken)
    {
        var command = new FindUserByUniversityId.Query(universityId);
        FindUserByUniversityId.Response user = await _mediator.Send(command, cancellationToken);

        return user.User;
    }
}