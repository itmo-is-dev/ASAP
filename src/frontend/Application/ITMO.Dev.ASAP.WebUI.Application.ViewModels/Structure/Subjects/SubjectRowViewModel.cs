using ITMO.Dev.ASAP.Application.Dto.Study;
using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Events.Subjects;
using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Messaging;
using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Structure.Subjects;
using System.Reactive.Linq;

namespace ITMO.Dev.ASAP.WebUI.Application.ViewModels.Structure.Subjects;

public class SubjectRowViewModel : ISubjectRowViewModel
{
    private readonly IMessageConsumer _consumer;

    public SubjectRowViewModel(
        IMessageConsumer consumer,
        IMessageProducer producer,
        SubjectDto subject)
    {
        Id = subject.Id;
        _consumer = consumer;

        Title = producer.Observe<SubjectUpdatedEvent>()
            .Where(x => x.Subject.Id.Equals(subject.Id))
            .Select(x => x.Subject.Title)
            .Prepend(subject.Title)
            .Replay(1)
            .AutoConnect();

        IsSelected = producer.Observe<SubjectSelectedEvent>()
            .Select(x => x.SubjectId.Equals(subject.Id))
            .Prepend(false)
            .Replay(1)
            .AutoConnect();
    }

    public Guid Id { get; }

    public IObservable<string> Title { get; }

    public IObservable<bool> IsSelected { get; }

    public ValueTask SelectAsync()
    {
        var evt = new SubjectSelectedEvent(Id);
        _consumer.Send(evt);

        return ValueTask.CompletedTask;
    }
}