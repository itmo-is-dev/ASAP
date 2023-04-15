using ITMO.Dev.ASAP.Application.Dto.Study;
using MediatR;

namespace ITMO.Dev.ASAP.Application.Contracts.Study.Submissions.Commands;

public static class CreateSubmission
{
    public record Command(Guid IssuerId, Guid StudentId, Guid AssignmentId, string Payload) : IRequest<Response>;

    public record Response(SubmissionDto Submission);
}