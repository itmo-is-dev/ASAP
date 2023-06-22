using ITMO.Dev.ASAP.Application.Dto.SubjectCourses;
using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Events.StudentGroups;
using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Events.SubjectCourses.Groups;
using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Events.SubjectCourses.Queues;
using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Messaging;
using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Structure.SubjectCourses.Queues;
using System.Reactive.Linq;

namespace ITMO.Dev.ASAP.WebUI.Application.ViewModels.Structure.SubjectCourses.Queues;

public class SubjectCourseQueueRow : ISubjectCourseQueueRow
{
    private readonly IMessageConsumer _consumer;

    public SubjectCourseQueueRow(
        SubjectCourseGroupDto subjectCourseGroup,
        IMessageConsumer consumer,
        IMessageProducer producer)
    {
        _consumer = consumer;

        SubjectCourseId = subjectCourseGroup.SubjectCourseId;
        StudentGroupId = subjectCourseGroup.StudentGroupId;

        IObservable<string> groupUpdated = producer.Observe<StudentGroupUpdatedEvent>()
            .Where(x => x.Group.Id.Equals(StudentGroupId))
            .Select(x => x.Group.Name);

        IObservable<string> subjectCourseGroupUpdated = producer.Observe<SubjectCourseGroupUpdatedEvent>()
            .Where(x => x.Group.SubjectCourseId.Equals(SubjectCourseId))
            .Where(x => x.Group.StudentGroupId.Equals(StudentGroupId))
            .Select(x => x.Group.StudentGroupName);

        GroupName = groupUpdated
            .Merge(subjectCourseGroupUpdated)
            .Prepend(subjectCourseGroup.StudentGroupName)
            .Replay(1)
            .AutoConnect();
    }

    public Guid SubjectCourseId { get; }

    public Guid StudentGroupId { get; }

    public IObservable<string> GroupName { get; }

    public void Select()
    {
        var evt = new SubjectCourseQueueSelectedEvent(SubjectCourseId, StudentGroupId);
        _consumer.Send(evt);
    }
}