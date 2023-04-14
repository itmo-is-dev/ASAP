namespace ITMO.Dev.ASAP.Presentation.Contracts.Services;

public interface IAsapPermissionService
{
    Task<bool> IsSubmissionMentorAsync(Guid userId, Guid submissionId, CancellationToken cancellationToken);
}