using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Events.SubjectCourses.Assignments;

namespace ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Structure.SubjectCourses.Assignments;

public interface ISubjectCourseAssignmentList
{
    IObservable<SubjectCourseAssignmentListUpdatedEvent> Assignments { get; }
}