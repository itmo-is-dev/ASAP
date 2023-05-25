using ITMO.Dev.ASAP.Authorization;
using ITMO.Dev.ASAP.Github.Application.Contracts.Users.Commands;
using ITMO.Dev.ASAP.Github.Application.Dto.Users;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ITMO.Dev.ASAP.Github.Presentation.Controllers;

[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private const string Scope = "Users";

    private readonly IMediator _mediator;

    public UserController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPut("{userId:guid}/github/username")]
    [AuthorizeFeature(Scope, nameof(GithubUpdateUsername))]
    public async Task<ActionResult<GithubUserDto>> GithubUpdateUsername(
        Guid userId,
        string value,
        CancellationToken cancellationToken)
    {
        var command = new UpdateGithubUser.Command(userId, value);
        UpdateGithubUser.Response response = await _mediator.Send(command, cancellationToken);

        return Ok(response);
    }
}