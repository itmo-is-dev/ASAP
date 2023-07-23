namespace ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Structure.SubjectCourses.Queues;

public interface ISubjectCourseQueueRow
{
    Guid SubjectCourseId { get; }

    Guid StudentGroupId { get; }

    IObservable<string> GroupName { get; }

    void Select();
}