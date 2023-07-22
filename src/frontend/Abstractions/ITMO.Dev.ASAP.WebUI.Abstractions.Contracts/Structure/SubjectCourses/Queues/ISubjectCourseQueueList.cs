namespace ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Structure.SubjectCourses.Queues;

public interface ISubjectCourseQueueList
{
    IObservable<IEnumerable<ISubjectCourseQueueRow>> Rows { get; }
}