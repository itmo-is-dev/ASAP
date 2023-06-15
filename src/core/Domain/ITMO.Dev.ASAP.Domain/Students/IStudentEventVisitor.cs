using ITMO.Dev.ASAP.Domain.Students.Events;

namespace ITMO.Dev.ASAP.Domain.Students;

public interface IStudentEventVisitor
{
    ValueTask VisitAsync(StudentTransferredEvent evt, CancellationToken cancellationToken);
}