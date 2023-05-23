using ITMO.Dev.ASAP.Application.Contracts.Identity.Commands;
using ITMO.Dev.ASAP.Application.Contracts.Identity.Queries;
using ITMO.Dev.ASAP.Application.Dto.Identity;
using ITMO.Dev.ASAP.Authorization;
using ITMO.Dev.ASAP.WebApi.Abstractions.Models.Identity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ITMO.Dev.ASAP.Controllers;

[ApiController]
[Route("api/identity")]
public class IdentityController : ControllerBase
{
    private const string Scope = "Identity";

    private readonly IMediator _mediator;

    public IdentityController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> LoginAsync([FromBody] LoginRequest request)
    {
        var query = new Login.Query(request.Username, request.Password);
        Login.Response response = await _mediator.Send(query, HttpContext.RequestAborted);

        var loginResponse = new LoginResponse(response.Token);
        return Ok(loginResponse);
    }

    [HttpPut("users/{username}/role")]
    [AuthorizeFeature(Scope, nameof(ChangeUserRole))]
    public async Task<IActionResult> ChangeUserRole(string username, [FromQuery] string roleName)
    {
        var command = new ChangeUserRole.Command(username, roleName);
        await _mediator.Send(command);

        return Ok();
    }

    [HttpPost("user/{id:guid}/create")]
    [AuthorizeFeature(Scope, nameof(CreateUserAccount))]
    public async Task<IActionResult> CreateUserAccount(Guid id, [FromBody] CreateUserAccountRequest request)
    {
        var command = new CreateUserAccount.Command(id, request.Username, request.Password, request.RoleName);
        await _mediator.Send(command);

        return Ok();
    }

    [HttpPut("username")]
    [Authorize]
    public async Task<ActionResult<UpdateUsernameResponse>> UpdateUsername(
        [FromBody] UpdateUsernameRequest request)
    {
        var updateCommand = new UpdateUsername.Command(request.Username);
        UpdateUsername.Response response = await _mediator.Send(updateCommand);

        return Ok(new UpdateUsernameResponse(response.Token));
    }

    [HttpPut("password")]
    [Authorize]
    public async Task<ActionResult<UpdatePasswordResponse>> UpdatePasswordAsync(
        [FromBody] UpdatePasswordRequest request)
    {
        var updateCommand = new UpdatePassword.Command(request.CurrentPassword, request.NewPassword);
        UpdatePassword.Response response = await _mediator.Send(updateCommand);

        return Ok(new UpdatePasswordResponse(response.Token));
    }

    [HttpGet("password/options")]
    public async Task<PasswordOptionsDto> GetPasswordOptionsAsync()
    {
        var query = new GetPasswordOptions.Query();
        GetPasswordOptions.Response response = await _mediator.Send(query, HttpContext.RequestAborted);

        return response.PasswordOptions;
    }
}