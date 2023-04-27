using ITMO.Dev.ASAP.Application.Dto.Study;

namespace ITMO.Dev.ASAP.WebUI.Abstractions.StudyNavigation;

public interface ISubjectProvider
{
    IObservable<Guid> Id { get; }

    IObservable<SubjectDto> Value { get; }

    Guid? CurrentId { get; }

    SubjectDto? CurrentValue { get; }

    void OnNext(Guid id);

    void OnNext(SubjectDto value);

    void Clear();
}