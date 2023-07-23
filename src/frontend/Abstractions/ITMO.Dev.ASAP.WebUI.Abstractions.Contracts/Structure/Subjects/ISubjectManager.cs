using ITMO.Dev.ASAP.Application.Dto.Study;
using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Events.Subjects;

namespace ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Structure.Subjects;

public interface ISubjectManager
{
    IObservable<SubjectCreatedEvent> SubjectCreated { get; }

    IObservable<SubjectDto> Subject { get; }

    ValueTask CreateAsync(string title, CancellationToken cancellationToken);

    ValueTask SelectAsync(Guid subjectId);
}