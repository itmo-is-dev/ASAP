using ITMO.Dev.ASAP.Application.Dto.Study;
using ITMO.Dev.ASAP.WebApi.Abstractions.Models.GroupAssignments;
using ITMO.Dev.ASAP.WebApi.Sdk.ControllerClients;
using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Events.GroupAssignments;
using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Messaging;
using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Structure.Assignments;
using ITMO.Dev.ASAP.WebUI.Abstractions.ExceptionHandling;
using ITMO.Dev.ASAP.WebUI.Abstractions.Extensions;
using ITMO.Dev.ASAP.WebUI.Abstractions.SafeExecution;
using System.Reactive.Linq;

namespace ITMO.Dev.ASAP.WebUI.Application.ViewModels.Structure.Assignments;

public class GroupAssignment : IGroupAssignment
{
    private readonly IMessageConsumer _consumer;
    private readonly ISafeExecutor _safeExecutor;
    private readonly IGroupAssignmentClient _groupAssignmentClient;

    public GroupAssignment(
        GroupAssignmentDto groupAssignment,
        IMessageProducer producer,
        IMessageConsumer consumer,
        ISafeExecutor safeExecutor,
        IGroupAssignmentClient groupAssignmentClient)
    {
        _consumer = consumer;
        _safeExecutor = safeExecutor;
        _groupAssignmentClient = groupAssignmentClient;

        GroupId = groupAssignment.GroupId;
        AssignmentId = groupAssignment.AssignmentId;

        GroupName = producer.Observe<GroupAssignmentUpdatedEvent>()
            .Where(x =>
                x.GroupAssignment.GroupId.Equals(GroupId) && x.GroupAssignment.AssignmentId.Equals(AssignmentId))
            .Select(x => x.GroupAssignment.GroupName)
            .Prepend(groupAssignment.GroupName)
            .Replay(1)
            .AutoConnect();

        AssignmentTitle = producer.Observe<GroupAssignmentUpdatedEvent>()
            .Where(x =>
                x.GroupAssignment.GroupId.Equals(GroupId) && x.GroupAssignment.AssignmentId.Equals(AssignmentId))
            .Select(x => x.GroupAssignment.AssignmentTitle)
            .Prepend(groupAssignment.AssignmentTitle)
            .Replay(1)
            .AutoConnect();

        Deadline = producer.Observe<GroupAssignmentUpdatedEvent>()
            .Where(x =>
                x.GroupAssignment.GroupId.Equals(GroupId) && x.GroupAssignment.AssignmentId.Equals(AssignmentId))
            .Select(x => x.GroupAssignment.Deadline)
            .Prepend(groupAssignment.Deadline)
            .Replay(1)
            .AutoConnect();
    }

    public Guid GroupId { get; }

    public Guid AssignmentId { get; }

    public IObservable<string> GroupName { get; }

    public IObservable<string> AssignmentTitle { get; }

    public IObservable<DateTime> Deadline { get; }

    public async Task<bool> UpdateDeadlineAsync(DateTime deadline, CancellationToken cancellationToken)
    {
        var tcs = new TaskCompletionSource<bool>();
        await ExecuteDeadlineUpdateAsync(tcs, deadline, cancellationToken);

        return await tcs.Task;
    }

    private async Task ExecuteDeadlineUpdateAsync(
        TaskCompletionSource<bool> tcs,
        DateTime deadline,
        CancellationToken cancellationToken)
    {
        await using ISafeExecutionBuilder<GroupAssignmentDto> builder = _safeExecutor.Execute(() =>
        {
            var request = new UpdateGroupAssignmentRequest(deadline);

            return _groupAssignmentClient.UpdateGroupAssignmentAsync(
                AssignmentId,
                GroupId,
                request,
                cancellationToken);
        });

        builder.Title = "Failed to update group assignment deadline";

        builder.OnSuccess(ga =>
        {
            var evt = new GroupAssignmentUpdatedEvent(ga);
            _consumer.Send(evt);
        });

        builder.OnSuccess(() => tcs.SetResult(true));
        builder.OnFail(() => tcs.SetResult(false));
    }
}