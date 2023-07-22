using ITMO.Dev.ASAP.Application.Dto.Study;
using ITMO.Dev.ASAP.WebApi.Sdk.ControllerClients;
using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Events.Assignments;
using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Events.GroupAssignments;
using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Messaging;
using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Structure.Assignments;
using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Tools;
using ITMO.Dev.ASAP.WebUI.Abstractions.ExceptionHandling;
using ITMO.Dev.ASAP.WebUI.Abstractions.Extensions;
using ITMO.Dev.ASAP.WebUI.Abstractions.SafeExecution;
using System.Reactive.Linq;

namespace ITMO.Dev.ASAP.WebUI.Application.ViewModels.Structure.Assignments;

public class Assignment : IAssignment, IDisposable
{
    private readonly IMessagePublisher _publisher;
    private readonly ISafeExecutor _safeExecutor;
    private readonly IAssignmentClient _assignmentClient;
    private readonly GroupAssignmentFactory _groupAssignmentFactory;

    private readonly IDisposable _subscription;

    private readonly List<IGroupAssignment> _groupAssignments;

    public Assignment(
        IMessageProvider provider,
        IMessagePublisher publisher,
        ISafeExecutor safeExecutor,
        IAssignmentClient assignmentClient,
        GroupAssignmentFactory groupAssignmentFactory)
    {
        _publisher = publisher;
        _safeExecutor = safeExecutor;
        _assignmentClient = assignmentClient;
        _groupAssignmentFactory = groupAssignmentFactory;

        _subscription = new SubscriptionBuilder()
            .Subscribe(provider.Observe<AssigmentSelectedEvent>().Subscribe(OnAssignmentSelected))
            .Build();

        _groupAssignments = new List<IGroupAssignment>();

        Title = provider.Observe<AssignmentUpdatedEvent>()
            .Where(x => x.Assignment.Id.Equals(Id))
            .Select(x => x.Assignment.Title)
            .Merge(provider.Observe<CurrentAssignmentLoadedEvent>().Select(x => x.Assignment.Title));

        MinPoints = provider.Observe<AssignmentUpdatedEvent>()
            .Where(x => x.Assignment.Id.Equals(Id))
            .Select(x => x.Assignment.MinPoints)
            .Merge(provider.Observe<CurrentAssignmentLoadedEvent>().Select(x => x.Assignment.MinPoints));

        MaxPoints = provider.Observe<AssignmentUpdatedEvent>()
            .Where(x => x.Assignment.Id.Equals(Id))
            .Select(x => x.Assignment.MaxPoints)
            .Merge(provider.Observe<CurrentAssignmentLoadedEvent>().Select(x => x.Assignment.MaxPoints));

        Visible = provider.Observe<AssignmentVisibleChangedEvent>().Select(x => x.IsVisible);

        GroupAssignments = provider.Observe<GroupAssignmentsListUpdatedEvent>();
    }

    public Guid Id { get; private set; }

    public IObservable<string> Title { get; }

    public IObservable<double> MinPoints { get; }

    public IObservable<double> MaxPoints { get; }

    public IObservable<bool> Visible { get; }

    public IObservable<GroupAssignmentsListUpdatedEvent> GroupAssignments { get; }

    public async ValueTask Update(double minPoints, double maxPoints, CancellationToken cancellationToken)
    {
        await using ISafeExecutionBuilder<AssignmentDto> builder = _safeExecutor
            .Execute(() => _assignmentClient.UpdateAssignmentPointsAsync(Id, minPoints, maxPoints, cancellationToken));

        builder.Title = "Failed to update assignment points";

        builder.OnSuccess(assignment =>
        {
            var evt = new AssignmentUpdatedEvent(assignment);
            _publisher.Send(evt);
        });
    }

    public void Dispose()
    {
        _subscription.Dispose();
    }

    private async void OnAssignmentSelected(AssigmentSelectedEvent evt)
    {
        await Task.WhenAll(LoadGroupAssignmentsAsync(evt.AssignmentId), UpdateAssignmentAsync(evt));
    }

    private async Task UpdateAssignmentAsync(AssigmentSelectedEvent evt)
    {
        await using ISafeExecutionBuilder<AssignmentDto> builder = _safeExecutor
            .Execute(() => _assignmentClient.GetByIdAsync(evt.AssignmentId));

        builder.Title = "Failed to load selected assignment";

        builder.OnSuccess(x => Id = x.Id);

        builder.OnSuccess(assignment =>
        {
            var loadedEvent = new CurrentAssignmentLoadedEvent(assignment);
            _publisher.Send(loadedEvent);
        });

        builder.OnSuccess(_ =>
        {
            var visibleEvent = new AssignmentVisibleChangedEvent(true);
            _publisher.Send(visibleEvent);
        });
    }

    private async Task LoadGroupAssignmentsAsync(Guid assignmentId)
    {
        await using ISafeExecutionBuilder<IReadOnlyCollection<GroupAssignmentDto>> builder = _safeExecutor
            .Execute(() => _assignmentClient.GetGroupAssignmentsAsync(assignmentId));

        builder.OnSuccess(groupAssignments =>
        {
            IEnumerable<Guid> existingIds = _groupAssignments.Select(x => x.GroupId);

            if (assignmentId != Id)
            {
                _groupAssignments.Clear();
            }
            else
            {
                IEnumerable<GroupAssignmentUpdatedEvent> toUpdate = groupAssignments
                    .IntersectBy(existingIds, x => x.GroupId)
                    .Select(x => new GroupAssignmentUpdatedEvent(x));

                _publisher.SendRange(toUpdate);
            }

            IEnumerable<IGroupAssignment> toAdd = groupAssignments
                .ExceptBy(existingIds, x => x.GroupId)
                .Select(_groupAssignmentFactory.Create);

            var groupIds = groupAssignments.Select(x => x.GroupId).ToHashSet();

            _groupAssignments.AddRange(toAdd);
            _groupAssignments.RemoveAll(x => groupIds.Contains(x.GroupId) is false);

            var evt = new GroupAssignmentsListUpdatedEvent(_groupAssignments);
            _publisher.Send(evt);
        });
    }
}