namespace ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Structure.SubjectCourses;

public interface ISubjectCourseRow
{
    Guid Id { get; }

    IObservable<string> Title { get; }

    IObservable<bool> IsSelected { get; }
}