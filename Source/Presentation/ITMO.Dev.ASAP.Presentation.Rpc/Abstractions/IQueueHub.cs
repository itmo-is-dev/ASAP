using ITMO.Dev.ASAP.Application.Dto.Tables;

namespace ITMO.Dev.ASAP.Presentation.Rpc.Abstractions;

public interface IQueueHub
{
    Task SendUpdateQueueMessage(SubmissionsQueueDto submissionsQueue, CancellationToken cancellationToken);
}