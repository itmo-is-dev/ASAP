using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Events.SubjectCourses.Queues;

namespace ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Structure.SubjectCourses.Queues;

public interface ISubjectCourseQueueList
{
    IObservable<SubjectCourseQueueListUpdatedEvent> Rows { get; }
}