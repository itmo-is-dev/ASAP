namespace ITMO.Dev.ASAP.WebUI.AdminPanel.Markup.StudyNavigation.Providers;

public interface ISubjectCourseProvider
{
    IObservable<Guid> Id { get; }

    Guid? Current { get; }

    void OnNext(Guid id);

    void Clear();
}