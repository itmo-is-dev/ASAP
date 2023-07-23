namespace ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Structure.SubjectCourses.Assignments;

public interface ISubjectCourseAssignmentList
{
    IObservable<IEnumerable<ISubjectCourseAssignmentRow>> Assignments { get; }
}