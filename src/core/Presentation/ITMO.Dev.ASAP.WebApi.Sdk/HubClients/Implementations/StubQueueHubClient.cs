using ITMO.Dev.ASAP.WebApi.Abstractions.Models.Queue;
using Microsoft.AspNetCore.SignalR.Client;
using System.Reactive.Subjects;

namespace ITMO.Dev.ASAP.WebApi.Sdk.HubClients.Implementations;

public class StubQueueHubClient : IQueueHubClient, IHubConnectionCreatable<IQueueHubClient>
{
    private readonly Subject<SubjectCourseQueueModel> _subject;

    public StubQueueHubClient()
    {
        _subject = new Subject<SubjectCourseQueueModel>();
    }

    public IObservable<SubjectCourseQueueModel> QueueUpdated => _subject;

    public static IQueueHubClient Create(HubConnection connection)
    {
        return new StubQueueHubClient();
    }

    public Task QueueUpdateSubscribeAsync(Guid courseId, Guid groupId, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public Task QueueUpdateUnsubscribeAsync(Guid courseId, Guid groupId, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public ValueTask DisposeAsync()
    {
        _subject.Dispose();
        return ValueTask.CompletedTask;
    }
}