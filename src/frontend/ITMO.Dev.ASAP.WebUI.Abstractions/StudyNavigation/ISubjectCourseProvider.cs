namespace ITMO.Dev.ASAP.WebUI.Abstractions.StudyNavigation;

public interface ISubjectCourseProvider
{
    IObservable<Guid> Id { get; }

    Guid? Current { get; }

    void OnNext(Guid id);

    void Clear();
}