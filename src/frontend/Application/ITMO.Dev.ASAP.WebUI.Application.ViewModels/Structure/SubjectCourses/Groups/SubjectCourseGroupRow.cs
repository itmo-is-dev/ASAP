using ITMO.Dev.ASAP.Application.Dto.SubjectCourses;
using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Events.StudentGroups;
using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Events.SubjectCourses.Groups;
using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Messaging;
using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Structure.SubjectCourses.Groups;
using System.Reactive.Linq;

namespace ITMO.Dev.ASAP.WebUI.Application.ViewModels.Structure.SubjectCourses.Groups;

public class SubjectCourseGroupRow : ISubjectCourseGroupRow
{
    private readonly IMessagePublisher _publisher;

    public SubjectCourseGroupRow(
        SubjectCourseGroupDto subjectCourseGroup,
        IMessageProvider provider,
        IMessagePublisher publisher)
    {
        _publisher = publisher;
        SubjectCourseId = subjectCourseGroup.SubjectCourseId;
        StudentGroupId = subjectCourseGroup.StudentGroupId;

        IObservable<string> groupUpdated = provider
            .Observe<StudentGroupUpdatedEvent>()
            .Where(x => x.Group.Id.Equals(StudentGroupId))
            .Select(x => x.Group.Name);

        IObservable<string> subjectCourseGroupUpdated = provider
            .Observe<SubjectCourseGroupUpdatedEvent>()
            .Where(x => x.Group.SubjectCourseId.Equals(SubjectCourseId))
            .Where(x => x.Group.StudentGroupId.Equals(StudentGroupId))
            .Select(x => x.Group.StudentGroupName);

        Title = groupUpdated
            .Merge(subjectCourseGroupUpdated)
            .Prepend(subjectCourseGroup.StudentGroupName)
            .Replay(1)
            .AutoConnect();
    }

    public Guid SubjectCourseId { get; }

    public Guid StudentGroupId { get; }

    public IObservable<string> Title { get; }

    public ValueTask SelectAsync(CancellationToken cancellationToken)
    {
        var evt = new StudentGroupSelectedEvent(StudentGroupId);
        _publisher.Send(evt);

        return ValueTask.CompletedTask;
    }
}