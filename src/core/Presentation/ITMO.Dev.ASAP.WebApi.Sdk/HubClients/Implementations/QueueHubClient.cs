using ITMO.Dev.ASAP.WebApi.Abstractions.Models.Queue;
using ITMO.Dev.ASAP.WebApi.Sdk.Errors;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using System.Reactive.Subjects;

namespace ITMO.Dev.ASAP.WebApi.Sdk.HubClients.Implementations;

public class QueueHubClient : IQueueHubClient, IHubConnectionCreatable<IQueueHubClient>
{
    private readonly Subject<SubjectCourseQueueModel> _queues;

    private readonly IDisposable _queueSubscription;
    private readonly IDisposable _errorSubscription;

    private readonly HubConnection _connection;

    public QueueHubClient(HubConnection connection, IExceptionSink exceptionSink)
    {
        _connection = connection;

        _queues = new Subject<SubjectCourseQueueModel>();

        _queueSubscription = _connection.On<SubjectCourseQueueModel>(
            "SendUpdateQueueMessage",
            x => _queues.OnNext(x));

        _errorSubscription = connection.On<string>(
            "SendError",
            async x => await exceptionSink.ConsumeAsync("Failed to execute queue request", x));
    }

    public IObservable<SubjectCourseQueueModel> QueueUpdated => _queues;

    public static IQueueHubClient Create(IServiceProvider provider, HubConnection connection)
    {
        IExceptionSink exceptionSink = provider.GetRequiredService<IExceptionSink>();
        return new QueueHubClient(connection, exceptionSink);
    }

    public async ValueTask DisposeAsync()
    {
        _queueSubscription.Dispose();
        _errorSubscription.Dispose();

        _queues.Dispose();

        await _connection.DisposeAsync();
    }

    public async Task QueueUpdateSubscribeAsync(Guid courseId, Guid groupId, CancellationToken cancellationToken)
    {
        await _connection.InvokeAsync("QueueUpdateSubscribeAsync", courseId, groupId, cancellationToken);
    }

    public async Task QueueUpdateUnsubscribeAsync(Guid courseId, Guid groupId, CancellationToken cancellationToken)
    {
        await _connection.InvokeAsync("QueueUpdateUnsubscribeAsync", courseId, groupId, cancellationToken);
    }
}