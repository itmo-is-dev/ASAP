namespace ITMO.Dev.ASAP.Domain.Students;

public interface IStudentEvent
{
    ValueTask AcceptAsync(IStudentEventVisitor visitor, CancellationToken cancellationToken);
}