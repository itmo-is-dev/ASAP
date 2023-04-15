using ITMO.Dev.ASAP.Application.Dto.Study;
using ITMO.Dev.ASAP.Application.Dto.Submissions;

namespace ITMO.Dev.ASAP.Presentation.Contracts.Services;

public interface IAsapSubmissionService
{
    Task<SubmissionDto> ActivateSubmissionAsync(Guid issuerId, Guid submissionId, CancellationToken cancellationToken);

    Task<SubmissionDto> BanSubmissionAsync(
        Guid issuerId,
        Guid studentId,
        Guid assignmentId,
        int? code,
        CancellationToken cancellationToken);

    Task<SubmissionDto> CreateSubmissionAsync(
        Guid issuerId,
        Guid userId,
        Guid assignmentId,
        string payload,
        CancellationToken cancellationToken);

    Task<SubmissionDto> DeactivateSubmissionAsync(Guid issuerId, Guid submissionId, CancellationToken cancellationToken);

    Task<SubmissionDto> DeleteSubmissionAsync(Guid issuerId, Guid submissionId, CancellationToken cancellationToken);

    Task<SubmissionDto> MarkSubmissionAsReviewedAsync(
        Guid issuerId,
        Guid submissionId,
        CancellationToken cancellationToken);

    Task<SubmissionRateDto> RateSubmissionAsync(
        Guid issuerId,
        Guid submissionId,
        double ratingPercent,
        double? extraPoints,
        CancellationToken cancellationToken);

    Task<SubmissionRateDto> UpdateSubmissionAsync(
        Guid issuerId,
        Guid userId,
        Guid assignmentId,
        int? code,
        DateOnly? dateTime,
        double? ratingPercent,
        double? extraPoints,
        CancellationToken cancellationToken);
}