using ITMO.Dev.ASAP.Application.Dto.Study;
using MediatR;

namespace ITMO.Dev.ASAP.Application.Contracts.Study.Submissions.Commands;

internal static class BanSubmission
{
    public record Command(Guid IssuerId, Guid UserId, Guid AssignmentId, int? Code) : IRequest<Response>;

    public record Response(SubmissionDto Submission);
}