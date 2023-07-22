using ITMO.Dev.ASAP.Application.Dto.Study;
using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Events.Assignments;
using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Messaging;
using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Structure.SubjectCourses.Assignments;
using System.Reactive.Linq;

namespace ITMO.Dev.ASAP.WebUI.Application.ViewModels.Structure.SubjectCourses.Assignments;

public class SubjectCourseAssignmentRow : ISubjectCourseAssignmentRow
{
    private readonly IMessagePublisher _publisher;

    public SubjectCourseAssignmentRow(
        AssignmentDto assignment,
        IMessageProvider provider,
        IMessagePublisher publisher)
    {
        _publisher = publisher;

        Id = assignment.Id;

        Title = provider
            .Observe<AssignmentUpdatedEvent>()
            .Where(x => x.Assignment.Id.Equals(Id))
            .Select(x => x.Assignment.Title)
            .Prepend(assignment.Title)
            .Replay(1)
            .AutoConnect();

        MinPoints = provider
            .Observe<AssignmentUpdatedEvent>()
            .Where(x => x.Assignment.Id.Equals(Id))
            .Select(x => x.Assignment.MinPoints)
            .Prepend(assignment.MinPoints)
            .Replay(1)
            .AutoConnect();

        MaxPoints = provider
            .Observe<AssignmentUpdatedEvent>()
            .Where(x => x.Assignment.Id.Equals(Id))
            .Select(x => x.Assignment.MaxPoints)
            .Prepend(assignment.MaxPoints)
            .Replay(1)
            .AutoConnect();
    }

    public Guid Id { get; }

    public IObservable<string> Title { get; }

    public IObservable<double> MinPoints { get; }

    public IObservable<double> MaxPoints { get; }

    public void Select()
    {
        var evt = new AssigmentSelectedEvent(Id);
        _publisher.Send(evt);
    }
}