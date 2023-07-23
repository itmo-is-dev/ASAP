namespace ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Structure.SubjectCourses;

public interface ISubjectCourseList
{
    IObservable<IEnumerable<ISubjectCourseRow>> SubjectCourses { get; }
}