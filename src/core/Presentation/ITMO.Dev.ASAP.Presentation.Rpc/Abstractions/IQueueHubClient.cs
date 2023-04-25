using ITMO.Dev.ASAP.WebApi.Abstractions.Models.Queue;

namespace ITMO.Dev.ASAP.Presentation.Rpc.Abstractions;

public interface IQueueHubClient
{
    Task SendUpdateQueueMessage(SubjectCourseQueueModel submissionsQueue, CancellationToken cancellationToken);

    Task SendError(string message);
}