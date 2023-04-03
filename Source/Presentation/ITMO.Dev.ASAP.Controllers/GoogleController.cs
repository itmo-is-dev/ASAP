using ITMO.Dev.ASAP.Application.Abstractions.Google;
using ITMO.Dev.ASAP.Application.Abstractions.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ITMO.Dev.ASAP.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = AsapIdentityRoleNames.AdminRoleName)]
public class GoogleController : ControllerBase
{
    private readonly ITableUpdateQueue _tableUpdate;

    public GoogleController(ITableUpdateQueue tableUpdate)
    {
        _tableUpdate = tableUpdate;
    }

    [HttpPost("force-sync")]
    public Task<ActionResult> ForceSubjectCourseTableSyncAsync(Guid subjectCourseId)
    {
        _tableUpdate.EnqueueCoursePointsUpdate(subjectCourseId);
        return Task.FromResult<ActionResult>(Ok());
    }
}