using ITMO.Dev.ASAP.Application.Dto.Submissions;
using MediatR;

namespace ITMO.Dev.ASAP.Application.Contracts.Study.Submissions.Commands;

internal static class UpdateSubmissionPoints
{
    public record Command(
        Guid IssuerId,
        Guid UserId,
        Guid AssignmentId,
        int? Code,
        double? RatingPercent,
        double? ExtraPoints) : IRequest<Response>;

    public record Response(SubmissionRateDto Submission);
}