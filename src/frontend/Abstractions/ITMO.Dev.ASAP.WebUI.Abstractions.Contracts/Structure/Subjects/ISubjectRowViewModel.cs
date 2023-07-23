namespace ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Structure.Subjects;

public interface ISubjectRowViewModel
{
    Guid Id { get; }

    IObservable<string> Title { get; }

    IObservable<bool> IsSelected { get; }

    ValueTask SelectAsync();
}