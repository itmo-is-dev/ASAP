using ITMO.Dev.ASAP.Application.Contracts.Study.Submissions.Commands;
using ITMO.Dev.ASAP.Application.Dto.Study;
using ITMO.Dev.ASAP.Application.Dto.Submissions;
using ITMO.Dev.ASAP.Presentation.Contracts.Services;
using MediatR;

namespace ITMO.Dev.ASAP.Presentation.Services.Implementations;

public class AsapSubmissionService : IAsapSubmissionService
{
    private readonly IMediator _mediator;

    public AsapSubmissionService(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<SubmissionDto> ActivateSubmissionAsync(
        Guid issuerId,
        Guid submissionId,
        CancellationToken cancellationToken)
    {
        var command = new ActivateSubmission.Command(submissionId);
        ActivateSubmission.Response response = await _mediator.Send(command, cancellationToken);

        return response.Submission;
    }

    public async Task<SubmissionDto> BanSubmissionAsync(
        Guid issuerId,
        Guid studentId,
        Guid assignmentId,
        int? code,
        CancellationToken cancellationToken)
    {
        var command = new BanSubmission.Command(issuerId, studentId, assignmentId, code);
        BanSubmission.Response response = await _mediator.Send(command, cancellationToken);

        return response.Submission;
    }

    public async Task<SubmissionDto> CreateSubmissionAsync(
        Guid issuerId,
        Guid userId,
        Guid assignmentId,
        string payload,
        CancellationToken cancellationToken)
    {
        var command = new CreateSubmission.Command(issuerId, userId, assignmentId, payload);
        CreateSubmission.Response response = await _mediator.Send(command, cancellationToken);

        return response.Submission;
    }

    public async Task<SubmissionDto> DeactivateSubmissionAsync(
        Guid issuerId,
        Guid submissionId,
        CancellationToken cancellationToken)
    {
        var command = new DeactivateSubmission.Command(submissionId);
        DeactivateSubmission.Response response = await _mediator.Send(command, cancellationToken);

        return response.Submission;
    }

    public async Task<SubmissionDto> DeleteSubmissionAsync(
        Guid issuerId,
        Guid submissionId,
        CancellationToken cancellationToken)
    {
        var command = new DeleteSubmission.Command(issuerId, submissionId);
        DeleteSubmission.Response response = await _mediator.Send(command, cancellationToken);

        return response.Submission;
    }

    public async Task<SubmissionDto> MarkSubmissionAsReviewedAsync(
        Guid issuerId,
        Guid submissionId,
        CancellationToken cancellationToken)
    {
        var command = new MarkSubmissionReviewed.Command(issuerId, submissionId);
        MarkSubmissionReviewed.Response response = await _mediator.Send(command, cancellationToken);

        return response.Submission;
    }

    public async Task<SubmissionRateDto> RateSubmissionAsync(
        Guid issuerId,
        Guid submissionId,
        double ratingPercent,
        double? extraPoints,
        CancellationToken cancellationToken)
    {
        var command = new RateSubmission.Command(issuerId, submissionId, ratingPercent, extraPoints);
        RateSubmission.Response response = await _mediator.Send(command, cancellationToken);

        return response.Submission;
    }

    public async Task<SubmissionRateDto> UpdateSubmissionAsync(
        Guid issuerId,
        Guid userId,
        Guid assignmentId,
        int? code,
        DateOnly? dateTime,
        double? ratingPercent,
        double? extraPoints,
        CancellationToken cancellationToken)
    {
        SubmissionRateDto? rateDto = null;

        if (ratingPercent is not null || extraPoints is not null)
        {
            var command = new UpdateSubmissionPoints.Command(
                issuerId,
                userId,
                assignmentId,
                code,
                ratingPercent,
                extraPoints);

            UpdateSubmissionPoints.Response response = await _mediator.Send(command, cancellationToken);

            rateDto = response.Submission;
        }

        if (dateTime is not null)
        {
            var command = new UpdateSubmissionDate.Command(
                issuerId,
                userId,
                assignmentId,
                code,
                dateTime.Value);

            UpdateSubmissionDate.Response response = await _mediator.Send(command, cancellationToken);

            rateDto = response.Submission;
        }

        return rateDto ?? throw new InvalidOperationException("No update command was executed");
    }
}