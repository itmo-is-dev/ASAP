using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Events.SubjectCourses;

namespace ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Structure.SubjectCourses;

public interface ISubjectCourseList
{
    IObservable<SubjectCourseListUpdatedEvent> SubjectCourses { get; }
}