namespace ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Structure.SubjectCourses.Groups;

public interface ISubjectCourseGroupRow
{
    Guid SubjectCourseId { get; }

    Guid StudentGroupId { get; }

    IObservable<string> Title { get; }

    ValueTask SelectAsync(CancellationToken cancellationToken);
}