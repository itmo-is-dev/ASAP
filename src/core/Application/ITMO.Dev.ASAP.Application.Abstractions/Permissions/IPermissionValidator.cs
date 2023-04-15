namespace ITMO.Dev.ASAP.Application.Abstractions.Permissions;

public interface IPermissionValidator
{
    Task<bool> IsOrganizationMentorAsync(Guid senderId, string organizationName, CancellationToken cancellationToken);

    Task<bool> IsSubmissionMentorAsync(Guid userId, Guid submissionId, CancellationToken cancellationToken);

    Task EnsureSubmissionMentorAsync(Guid userId, Guid submissionId, CancellationToken cancellationToken);
}