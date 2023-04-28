using ITMO.Dev.ASAP.Application.Abstractions.SubjectCourses;
using ITMO.Dev.ASAP.Application.Tools;

namespace ITMO.Dev.ASAP.Application.SubjectCourses;

public class SubjectCourseUpdater : ISubjectCourseUpdateService
{
    private readonly ConcurrentHashSet<Guid> _pointUpdates;

    public SubjectCourseUpdater()
    {
        _pointUpdates = new ConcurrentHashSet<Guid>();
    }

    public IReadOnlyCollection<Guid> PointUpdates => _pointUpdates.GetAndClearValues();

    public void UpdatePoints(Guid subjectCourseId)
    {
        _pointUpdates.Add(subjectCourseId);
    }
}