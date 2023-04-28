namespace ITMO.Dev.ASAP.Application.Abstractions.Queue;

public interface IQueueUpdateService
{
    void Update(Guid subjectCourseId, Guid studentGroupId);
}