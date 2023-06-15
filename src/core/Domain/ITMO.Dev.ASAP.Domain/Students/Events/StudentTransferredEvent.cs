namespace ITMO.Dev.ASAP.Domain.Students.Events;

public class StudentTransferredEvent : IStudentEvent
{
    public StudentTransferredEvent(Student student)
    {
        Student = student;
    }

    public Student Student { get; }

    public ValueTask AcceptAsync(IStudentEventVisitor visitor, CancellationToken cancellationToken)
    {
        return visitor.VisitAsync(this, cancellationToken);
    }
}