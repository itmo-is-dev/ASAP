using System.Reactive.Subjects;

namespace ITMO.Dev.ASAP.WebUI.AdminPanel.Markup.StudyNavigation.Providers;

public class SubjectCourseProvider : ISubjectCourseProvider, IDisposable
{
    private readonly Subject<Guid> _subject;

    public SubjectCourseProvider()
    {
        _subject = new Subject<Guid>();
    }

    public IObservable<Guid> Id => _subject;

    public Guid? Current { get; private set; }

    public void OnNext(Guid id)
    {
        if (id.Equals(Current))
            return;

        Current = id;
        _subject.OnNext(id);
    }

    public void Clear()
    {
        Current = null;
    }

    public void Dispose()
        => _subject.Dispose();
}