using ITMO.Dev.ASAP.WebApi.Abstractions.Models.Queue;

namespace ITMO.Dev.ASAP.WebApi.Sdk.HubClients;

public interface IQueueHubClient : IHubClient
{
    IObservable<SubjectCourseQueueModel> QueueUpdated { get; }

    Task QueueUpdateSubscribeAsync(Guid courseId, Guid groupId, CancellationToken cancellationToken);

    Task QueueUpdateUnsubscribeAsync(Guid courseId, Guid groupId, CancellationToken cancellationToken);
}