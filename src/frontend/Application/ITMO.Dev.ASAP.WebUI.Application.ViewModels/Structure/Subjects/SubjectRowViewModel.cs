using ITMO.Dev.ASAP.Application.Dto.Study;
using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Events.SubjectCourses;
using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Events.Subjects;
using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Messaging;
using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Structure.Subjects;
using System.Reactive.Linq;

namespace ITMO.Dev.ASAP.WebUI.Application.ViewModels.Structure.Subjects;

public class SubjectRowViewModel : ISubjectRowViewModel
{
    private readonly IMessagePublisher _publisher;

    public SubjectRowViewModel(
        IMessagePublisher publisher,
        IMessageProvider provider,
        SubjectDto subject)
    {
        Id = subject.Id;
        _publisher = publisher;

        Title = provider
            .Observe<SubjectUpdatedEvent>()
            .Where(x => x.Subject.Id.Equals(subject.Id))
            .Select(x => x.Subject.Title)
            .Prepend(subject.Title)
            .Replay(1)
            .AutoConnect();

        IsSelected = provider
            .Observe<SubjectSelectedEvent>()
            .Select(x => x.SubjectId.Equals(subject.Id))
            .Prepend(false);
    }

    public Guid Id { get; }

    public IObservable<string> Title { get; }

    public IObservable<bool> IsSelected { get; }

    public ValueTask SelectAsync()
    {
        var evt = new SubjectSelectedEvent(Id);
        _publisher.Send(evt);

        var subjectCourseSelectedEvent = new SubjectCourseSelectedEvent(null);
        _publisher.Send(subjectCourseSelectedEvent);

        return ValueTask.CompletedTask;
    }
}