using ITMO.Dev.ASAP.Application.Dto.Submissions;
using ITMO.Dev.ASAP.Domain.ValueObject;
using Submission = ITMO.Dev.ASAP.Domain.Submissions.Submission;

namespace ITMO.Dev.ASAP.Application.Factories;

public static class SubmissionRateDtoFactory
{
    public static SubmissionRateDto CreateFromSubmission(Submission submission)
    {
        ArgumentNullException.ThrowIfNull(submission);

        Points maxRowPoints = submission.GroupAssignment.Assignment.MaxPoints;

        double? rating = null;
        if (submission.Rating is not null)
            rating = submission.Rating * 100;

        var dto = new SubmissionRateDto(
            submission.Id,
            submission.Code,
            submission.State.Kind.ToString(),
            submission.SubmissionDate.Value,
            rating,
            submission.Points?.Value,
            maxRowPoints.Value,
            submission.ExtraPoints?.Value,
            submission.PointPenalty?.Value,
            submission.EffectivePoints?.Value);

        return dto;
    }
}