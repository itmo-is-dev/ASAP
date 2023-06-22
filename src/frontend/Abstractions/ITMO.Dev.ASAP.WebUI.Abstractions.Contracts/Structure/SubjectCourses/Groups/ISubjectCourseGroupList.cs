using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Events.SubjectCourses.Groups;

namespace ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Structure.SubjectCourses.Groups;

public interface ISubjectCourseGroupList
{
    IObservable<SubjectCourseGroupListUpdatedEvent> SubjectCourseGroups { get; }

    IObservable<bool> AddSubjectCourseGroupsVisible { get; }

    void ShowAddSubjectCourseGroups();

    ValueTask AddAsync(IReadOnlyCollection<Guid> studentGroupIds, CancellationToken cancellationToken);
}