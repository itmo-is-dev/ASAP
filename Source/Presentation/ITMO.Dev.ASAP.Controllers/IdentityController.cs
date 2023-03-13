using ITMO.Dev.ASAP.Application.Contracts.Identity.Commands;
using ITMO.Dev.ASAP.Application.Contracts.Identity.Queries;
using ITMO.Dev.ASAP.Identity.Entities;
using ITMO.Dev.ASAP.WebApi.Abstractions.Models.Identity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ITMO.Dev.ASAP.Controllers;

[ApiController]
[Route("api/identity")]
public class IdentityController : ControllerBase
{
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

        var loginResponse = new LoginResponse(response.Token, response.Expires, response.Roles);
        return Ok(loginResponse);
    }

    [HttpPost("users/{username}/role")]
    [Authorize(Roles = $"{AsapIdentityRole.AdminRoleName}, {AsapIdentityRole.ModeratorRoleName}")]
    public async Task<IActionResult> ChangeUserRoleAsync(string username, [FromBody] ChangeUserRoleRequest request)
    {
        var command = new ChangeUserRole.Command(username, request.RoleName);
        await _mediator.Send(command);

        return Ok();
    }

    [HttpPost("register")]
    [Authorize(Roles = AsapIdentityRole.AdminRoleName)]
    public async Task<ActionResult<LoginResponse>> RegisterAsync([FromBody] RegisterUserRequest request)
    {
        var registerCommand = new Register.Command(request.Username, request.Password);
        await _mediator.Send(registerCommand);

        var loginCommand = new Login.Query(request.Username, request.Password);
        Login.Response loginResponse = await _mediator.Send(loginCommand, HttpContext.RequestAborted);

        var credentials = new LoginResponse(loginResponse.Token, loginResponse.Expires, loginResponse.Roles);
        return Ok(credentials);
    }

    [HttpPut("user/{id:guid}/create")]
    [Authorize(Roles = $"{AsapIdentityRole.AdminRoleName}, {AsapIdentityRole.ModeratorRoleName}")]
    public async Task<IActionResult> CreateUserAccountAsync(Guid id, [FromBody] CreateUserAccountRequest request)
    {
        var command = new CreateUserAccount.Command(id, request.Username, request.Password);
        await _mediator.Send(command);

        return Ok();
    }
}