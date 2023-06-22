using ITMO.Dev.ASAP.Application.Dto.Study;
using ITMO.Dev.ASAP.WebApi.Sdk.ControllerClients;
using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Events.Subjects;
using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Messaging;
using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Structure.Subjects;
using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Tools;
using ITMO.Dev.ASAP.WebUI.Abstractions.ExceptionHandling;
using ITMO.Dev.ASAP.WebUI.Abstractions.Extensions;
using ITMO.Dev.ASAP.WebUI.Abstractions.SafeExecution;

namespace ITMO.Dev.ASAP.WebUI.Application.ViewModels.Structure.Subjects;

public class SubjectList : ISubjectList, IDisposable
{
    private readonly List<ISubjectRowViewModel> _subjectViewModels;
    private readonly ISubjectClient _subjectClient;
    private readonly IMessageConsumer _consumer;
    private readonly IMessageProducer _producer;
    private readonly IDisposable _subscription;
    private readonly ISafeExecutor _safeExecutor;

    public SubjectList(
        ISubjectClient subjectClient,
        IMessageConsumer consumer,
        IMessageProducer producer,
        ISafeExecutor safeExecutor)
    {
        _subjectClient = subjectClient;
        _consumer = consumer;
        _producer = producer;
        _safeExecutor = safeExecutor;

        _subjectViewModels = new List<ISubjectRowViewModel>();

        _subscription = new SubscriptionBuilder()
            .Subscribe(producer.Observe<SubjectCreatedEvent>().Subscribe(OnSubjectCreated))
            .Build();
    }

    public IObservable<SubjectListUpdatedEvent> Subjects => _producer.Observe<SubjectListUpdatedEvent>();

    public async ValueTask LoadAsync(CancellationToken cancellationToken)
    {
        await using ISafeExecutionBuilder<IReadOnlyCollection<SubjectDto>> builder = _safeExecutor
            .Execute(() => _subjectClient.GetAsync(cancellationToken));

        builder.Title = "Failed to load subjects";

        builder.OnSuccess(subjects =>
        {
            IEnumerable<Guid> ids = _subjectViewModels.Select(x => x.Id);

            IEnumerable<SubjectUpdatedEvent> subjectsToUpdate = subjects
                .IntersectBy(ids, x => x.Id)
                .Select(x => new SubjectUpdatedEvent(x));

            IEnumerable<ISubjectRowViewModel> subjectsToAdd = subjects
                .ExceptBy(ids, x => x.Id)
                .Select(x => new SubjectRowViewModel(_consumer, _producer, x));

            _consumer.SendRange(subjectsToUpdate);
            _subjectViewModels.AddRange(subjectsToAdd);

            var evt = new SubjectListUpdatedEvent(_subjectViewModels);
            _consumer.Send(evt);
        });
    }

    public void Dispose()
    {
        _subscription.Dispose();
    }

    private void OnSubjectCreated(SubjectCreatedEvent evt)
    {
        var viewModel = new SubjectRowViewModel(_consumer, _producer, evt.Subject);
        _subjectViewModels.Add(viewModel);

        var updatedEvt = new SubjectListUpdatedEvent(_subjectViewModels);
        _consumer.Send(updatedEvt);
    }
}