using ITMO.Dev.ASAP.Application.Abstractions.Queue;
using ITMO.Dev.ASAP.Application.Tools;

namespace ITMO.Dev.ASAP.Application.Queue;

public class QueueUpdater : IQueueUpdateService
{
    private readonly ConcurrentHashSet<(Guid CourseId, Guid GroupId)> _set;

    public QueueUpdater()
    {
        _set = new ConcurrentHashSet<(Guid, Guid)>();
    }

    public IReadOnlyCollection<(Guid CourseId, Guid GroupId)> Values => _set.GetAndClearValues();

    public void Update(Guid subjectCourseId, Guid studentGroupId)
    {
        _set.Add((subjectCourseId, studentGroupId));
    }
}