using ITMO.Dev.ASAP.Application.Dto.Study;
using System.Reactive.Subjects;

namespace ITMO.Dev.ASAP.WebUI.AdminPanel.Markup.StudyNavigation.Providers;

public class SubjectProvider : ISubjectProvider, IDisposable
{
    private readonly Subject<Guid> _idSubject;
    private readonly Subject<SubjectDto> _valueSubject;

    public SubjectProvider()
    {
        _idSubject = new Subject<Guid>();
        _valueSubject = new Subject<SubjectDto>();
    }

    public IObservable<Guid> Id => _idSubject;

    public IObservable<SubjectDto> Value => _valueSubject;

    public Guid? CurrentId { get; private set; }

    public SubjectDto? CurrentValue { get; private set; }

    public void OnNext(Guid id)
    {
        if (id.Equals(CurrentId))
            return;

        CurrentId = id;
        _idSubject.OnNext(id);
    }

    public void OnNext(SubjectDto value)
    {
        CurrentValue = value;
        _valueSubject.OnNext(value);
    }

    public void Clear()
    {
        CurrentId = null;
        CurrentValue = null;
    }

    public void Dispose()
    {
        _idSubject.Dispose();
        _valueSubject.Dispose();
    }
}