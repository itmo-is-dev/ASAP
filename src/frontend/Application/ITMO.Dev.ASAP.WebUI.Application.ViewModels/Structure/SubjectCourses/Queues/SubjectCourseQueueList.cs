using ITMO.Dev.ASAP.Application.Dto.SubjectCourses;
using ITMO.Dev.ASAP.WebApi.Sdk.ControllerClients;
using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Events.SubjectCourses;
using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Events.SubjectCourses.Groups;
using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Events.SubjectCourses.Queues;
using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Messaging;
using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Models;
using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Structure.SubjectCourses.Queues;
using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Tools;
using ITMO.Dev.ASAP.WebUI.Abstractions.ExceptionHandling;
using ITMO.Dev.ASAP.WebUI.Abstractions.Extensions;
using ITMO.Dev.ASAP.WebUI.Abstractions.SafeExecution;
using Microsoft.Extensions.Logging;

namespace ITMO.Dev.ASAP.WebUI.Application.ViewModels.Structure.SubjectCourses.Queues;

public class SubjectCourseQueueList : ISubjectCourseQueueList, IDisposable
{
    private readonly IMessageConsumer _consumer;
    private readonly IMessageProducer _producer;
    private readonly ILogger<SubjectCourseQueueList> _logger;
    private readonly ISafeExecutor _safeExecutor;
    private readonly ISubjectCourseClient _subjectCourseClient;

    private readonly IDisposable _subscription;
    private readonly List<ISubjectCourseQueueRow> _rows;

    private Guid? _subjectCourseId;
    private bool _isSelected;

    public SubjectCourseQueueList(
        IMessageConsumer consumer,
        IMessageProducer producer,
        ILogger<SubjectCourseQueueList> logger,
        ISafeExecutor safeExecutor,
        ISubjectCourseClient subjectCourseClient)
    {
        _consumer = consumer;
        _producer = producer;
        _logger = logger;
        _safeExecutor = safeExecutor;
        _subjectCourseClient = subjectCourseClient;

        _subscription = new SubscriptionBuilder()
            .Subscribe(producer.Observe<SubjectCourseSelectedEvent>().Subscribe(OnSubjectCourseSelected))
            .Subscribe(producer.Observe<SubjectCourseSelectionUpdatedEvent>()
                .Subscribe(OnSubjectCourseSelectionUpdated))
            .Build();

        _rows = new List<ISubjectCourseQueueRow>();

        Rows = producer.Observe<SubjectCourseQueueListUpdatedEvent>();
    }

    public IObservable<SubjectCourseQueueListUpdatedEvent> Rows { get; }

    public void Dispose()
    {
        _subscription.Dispose();
    }

    private async void OnSubjectCourseSelected(SubjectCourseSelectedEvent evt)
    {
        _subjectCourseId = evt.SubjectCourseId;

        if (_isSelected)
        {
            await ReloadSubjectCourseGroupsAsync();
        }
    }

    private async void OnSubjectCourseSelectionUpdated(SubjectCourseSelectionUpdatedEvent evt)
    {
        _isSelected = evt.Selection is SubjectCourseSelection.Queues;

        if (_isSelected)
        {
            await ReloadSubjectCourseGroupsAsync();
        }
    }

    private async ValueTask ReloadSubjectCourseGroupsAsync()
    {
        if (_subjectCourseId is null)
        {
            _logger.LogWarning("No subject course selected");
            return;
        }

        await using ISafeExecutionBuilder<IReadOnlyCollection<SubjectCourseGroupDto>> builder = _safeExecutor
            .Execute(() => _subjectCourseClient.GetGroupsAsync(_subjectCourseId.Value));

        builder.Title = "Failed to load subject course groups";

        builder.OnSuccess(assignments =>
        {
            IEnumerable<(Guid SubjectCourseId, Guid StudentGroupId)> existingIds = _rows
                .Select(x => (x.SubjectCourseId, x.StudentGroupId));

            IEnumerable<SubjectCourseGroupUpdatedEvent> assignmentsToUpdate = assignments
                .IntersectBy(existingIds, x => (x.SubjectCourseId, x.StudentGroupId))
                .Select(x => new SubjectCourseGroupUpdatedEvent(x));

            IEnumerable<SubjectCourseQueueRow> assignmentsToAdd = assignments
                .ExceptBy(existingIds, x => (x.SubjectCourseId, x.StudentGroupId))
                .Select(x => new SubjectCourseQueueRow(x, _consumer, _producer));

            var assignmentIds = assignments
                .Select(x => (x.SubjectCourseId, x.StudentGroupId))
                .ToHashSet();

            _consumer.SendRange(assignmentsToUpdate);
            _rows.AddRange(assignmentsToAdd);
            _rows.RemoveAll(x => assignmentIds.Contains((x.SubjectCourseId, x.StudentGroupId)) is false);

            var evt = new SubjectCourseQueueListUpdatedEvent(_rows);
            _consumer.Send(evt);
        });
    }
}