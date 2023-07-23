using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Events.Subjects;

namespace ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Structure.Subjects;

public interface ISubjectList
{
    IObservable<SubjectListUpdatedEvent> Subjects { get; }

    ValueTask LoadAsync(CancellationToken cancellationToken);
}