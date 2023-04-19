using ITMO.Dev.ASAP.WebApi.Abstractions.Models.Queue;
using Microsoft.AspNetCore.SignalR.Client;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace ITMO.Dev.ASAP.WebApi.Sdk.HubClients.Implementations;

public class QueueHubClient : IQueueHubClient, IHubConnectionCreatable<IQueueHubClient>
{
    private readonly Subject<SubjectCourseQueueModel> _subject;

    private readonly HubConnection _connection;

    private readonly IDisposable _updateHandler;

    public QueueHubClient(HubConnection connection)
    {
        _connection = connection;
        _subject = new Subject<SubjectCourseQueueModel>();

        _updateHandler = _connection.On<SubjectCourseQueueModel>(
            "SendUpdateQueueMessage",
            x => _subject.OnNext(x));
    }

    public IObservable<SubjectCourseQueueModel> QueueUpdated => _subject.AsObservable();

    public static IQueueHubClient Create(HubConnection connection)
    {
        return new QueueHubClient(connection);
    }

    public async ValueTask DisposeAsync()
    {
        await _connection.DisposeAsync();
        _updateHandler.Dispose();
        _subject.Dispose();
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