using ITMO.Dev.ASAP.Application.Dto.Tables;
using ITMO.Dev.ASAP.WebApi.Sdk.ControllerClients;
using ITMO.Dev.ASAP.WebApi.Sdk.HubClients;
using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Events.SubjectCourses.Queues;
using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Messaging;
using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Structure.SubjectCourses.Queues;
using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Tools;
using ITMO.Dev.ASAP.WebUI.Abstractions.ExceptionHandling;
using ITMO.Dev.ASAP.WebUI.Abstractions.Extensions;
using ITMO.Dev.ASAP.WebUI.Abstractions.SafeExecution;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace ITMO.Dev.ASAP.WebUI.Application.ViewModels.Structure.SubjectCourses.Queues;

public class SubjectCourseQueue : ISubjectCourseQueue, IAsyncDisposable
{
    private readonly IMessageConsumer _consumer;
    private readonly ISafeExecutor _safeExecutor;
    private readonly ISubjectCourseClient _subjectCourseClient;
    private readonly IHubClientProvider<IQueueHubClient> _clientProvider;

    private readonly IDisposable _subscription;
    private IDisposable _queueUpdateSubscription;
    private SubjectCourseQueueSelectedEvent? _latest;

    public SubjectCourseQueue(
        IMessageConsumer consumer,
        IMessageProducer producer,
        ISafeExecutor safeExecutor,
        ISubjectCourseClient subjectCourseClient,
        IHubClientProvider<IQueueHubClient> clientProvider)
    {
        _consumer = consumer;
        _safeExecutor = safeExecutor;
        _subjectCourseClient = subjectCourseClient;
        _clientProvider = clientProvider;

        _queueUpdateSubscription = Disposable.Empty;

        _subscription = new SubscriptionBuilder()
            .Subscribe(producer.Observe<SubjectCourseQueueSelectedEvent>().Subscribe(OnQueueSelected))
            .Build();

        Queue = producer.Observe<SubjectCourseQueueLoadedEvent>().Select(x => x.Queue);
    }

    public IObservable<SubmissionsQueueDto> Queue { get; }

    public ValueTask UnsubscribeAsync(CancellationToken cancellationToken)
    {
        return UnsubscribeFromQueueAsync();
    }

    public async ValueTask DisposeAsync()
    {
        _subscription.Dispose();
        await UnsubscribeFromQueueAsync();
    }

    private async void OnQueueSelected(SubjectCourseQueueSelectedEvent evt)
    {
        await UnsubscribeFromQueueAsync();
        await LoadQueueAsync(evt);
        await SubscribeToQueueAsync(evt);
    }

    private void OnQueueLoaded(SubmissionsQueueDto queue)
    {
        var queueEvent = new SubjectCourseQueueLoadedEvent(queue);
        _consumer.Send(queueEvent);
    }

    private async Task LoadQueueAsync(SubjectCourseQueueSelectedEvent evt)
    {
        await using ISafeExecutionBuilder<SubmissionsQueueDto> builder = _safeExecutor.Execute(() =>
        {
            Guid subjectCourseId = evt.SubjectCourseId;
            Guid studentGroupId = evt.StudentGroupId;

            return _subjectCourseClient.GetStudyGroupQueueAsync(subjectCourseId, studentGroupId);
        });

        builder.Title = "Failed to load queue";
        builder.OnSuccess(OnQueueLoaded);
    }

    private async Task SubscribeToQueueAsync(SubjectCourseQueueSelectedEvent evt)
    {
        await using ISafeExecutionBuilder builder = _safeExecutor.Execute(async () =>
        {
            IQueueHubClient client = await _clientProvider.GetClientAsync();

            Guid subjectCourseId = evt.SubjectCourseId;
            Guid studentGroupId = evt.StudentGroupId;

            await client.QueueUpdateSubscribeAsync(subjectCourseId, studentGroupId, default);
        });

        builder.OnSuccessAsync(async () =>
        {
            IQueueHubClient client = await _clientProvider.GetClientAsync();

            _queueUpdateSubscription = client.QueueUpdated
                .Where(x => x.SubjectCourseId.Equals(evt.SubjectCourseId))
                .Where(x => x.StudyGroupId.Equals(evt.StudentGroupId))
                .Select(x => x.Queue)
                .Subscribe(OnQueueLoaded);

            _latest = evt;
        });
    }

    private async ValueTask UnsubscribeFromQueueAsync()
    {
        if (_latest is null)
            return;

        await using ISafeExecutionBuilder builder = _safeExecutor.Execute(async () =>
        {
            IQueueHubClient client = await _clientProvider.GetClientAsync();

            await client.QueueUpdateUnsubscribeAsync(
                _latest.SubjectCourseId,
                _latest.StudentGroupId,
                default);
        });

        builder.OnSuccess(() => _queueUpdateSubscription.Dispose());
        builder.OnSuccess(() => _latest = null);
    }
}