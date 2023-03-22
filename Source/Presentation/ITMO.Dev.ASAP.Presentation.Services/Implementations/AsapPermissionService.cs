using ITMO.Dev.ASAP.Application.Abstractions.Permissions;
using ITMO.Dev.ASAP.Presentation.Contracts.Services;

namespace ITMO.Dev.ASAP.Presentation.Services.Implementations;

public class AsapPermissionService : IAsapPermissionService
{
    private readonly IPermissionValidator _permissionValidator;

    public AsapPermissionService(IPermissionValidator permissionValidator)
    {
        _permissionValidator = permissionValidator;
    }

    public async Task<bool> IsSubmissionMentorAsync(Guid userId, Guid submissionId, CancellationToken cancellationToken)
    {
        return await _permissionValidator.IsSubmissionMentorAsync(userId, submissionId, cancellationToken);
    }
}