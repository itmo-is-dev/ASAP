using ITMO.Dev.ASAP.DeveloperEnvironment;
using ITMO.Dev.ASAP.Application.Abstractions.Identity;
using ITMO.Dev.ASAP.Application.GithubWorkflow.Abstractions.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ITMO.Dev.ASAP.Controllers;

[ApiController]
[Route("api/internal/")]
[Authorize(Roles = AsapIdentityRoleNames.AdminRoleName)]
public class InternalController : ControllerBase
{
    private readonly DeveloperEnvironmentSeeder _developerEnvironmentSeeder;

    public InternalController(
        DeveloperEnvironmentSeeder developerEnvironmentSeeder)
    {
        _developerEnvironmentSeeder = developerEnvironmentSeeder;
    }

    [HttpPost("seed-test-data")]
    public async Task<IActionResult> SeedTestData([FromQuery] string environment)
    {
        var command = new DeveloperEnvironmentSeedingRequest(environment);

        try
        {
            await _developerEnvironmentSeeder.Handle(command, CancellationToken.None);
        }
        catch (UserNotAcknowledgedEnvironmentException e)
        {
            return StatusCode(
                (int)HttpStatusCode.BadRequest,
                new ProblemDetails
                {
                    Status = (int)HttpStatusCode.BadRequest,
                    Title = e.Message,
                    Detail =
                        "You must put string 'Testing' into environment field if you have 100% ensured that it is not a production environment",
                });
        }

        return NoContent();
    }
}