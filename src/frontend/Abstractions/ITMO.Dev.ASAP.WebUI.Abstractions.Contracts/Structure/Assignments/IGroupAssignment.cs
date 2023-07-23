namespace ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Structure.Assignments;

public interface IGroupAssignment
{
    Guid GroupId { get; }

    Guid AssignmentId { get; }

    IObservable<string> GroupName { get; }

    IObservable<string> AssignmentTitle { get; }

    IObservable<DateTime> Deadline { get; }

    Task<bool> UpdateDeadlineAsync(DateTime deadline, CancellationToken cancellationToken);
}