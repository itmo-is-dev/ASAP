namespace ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Structure.SubjectCourses.Assignments;

public interface ISubjectCourseAssignmentRow
{
    Guid Id { get; }

    IObservable<string> Title { get; }

    IObservable<double> MinPoints { get; }

    IObservable<double> MaxPoints { get; }

    void Select();
}