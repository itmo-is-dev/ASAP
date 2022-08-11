using MediatR;

namespace Kysect.Shreks.Application.Abstractions.Submissions.Commands;

public static class CreateSubmissionCommand
{
    public record Command(Guid StudentId, Guid AssignmentId, string Payload) : IRequest<Response>;

    public record Response(Guid SubmissionId);
}