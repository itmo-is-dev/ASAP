using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Events.Assignments;

namespace ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Structure.Assignments;

public interface IAssignment
{
    Guid Id { get; }

    IObservable<string> Title { get; }

    IObservable<double> MinPoints { get; }

    IObservable<double> MaxPoints { get; }

    IObservable<bool> Visible { get; }

    IObservable<GroupAssignmentsListUpdatedEvent> GroupAssignments { get; }

    ValueTask Update(double minPoints, double maxPoints, CancellationToken cancellationToken);
}