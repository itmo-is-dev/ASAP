using ITMO.Dev.ASAP.Application.Dto.Submissions;
using MediatR;

namespace ITMO.Dev.ASAP.Application.Contracts.Study.Submissions.Commands;

internal static class UpdateSubmissionDate
{
    public record Command(
        Guid IssuerId,
        Guid UserId,
        Guid AssignmentId,
        int? Code,
        DateOnly Date) : IRequest<Response>;

    public record Response(SubmissionRateDto Submission);
}