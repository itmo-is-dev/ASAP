using ITMO.Dev.ASAP.Application.Dto.Tables;

namespace ITMO.Dev.ASAP.Application.Abstractions.Queue;

public interface IQueueUpdateService
{
    Task<SubmissionsQueueDto> GetSubmissionsQueue(Guid subjectCourseId, Guid studentGroupId, CancellationToken cancellationToken);
}