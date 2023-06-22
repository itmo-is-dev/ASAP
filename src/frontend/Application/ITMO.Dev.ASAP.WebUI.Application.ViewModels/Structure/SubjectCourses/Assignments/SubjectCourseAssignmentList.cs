using ITMO.Dev.ASAP.Application.Dto.Study;
using ITMO.Dev.ASAP.WebApi.Sdk.ControllerClients;
using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Events.Assignments;
using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Events.SubjectCourses;
using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Events.SubjectCourses.Assignments;
using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Messaging;
using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Models;
using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Structure.SubjectCourses.Assignments;
using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Tools;
using ITMO.Dev.ASAP.WebUI.Abstractions.ExceptionHandling;
using ITMO.Dev.ASAP.WebUI.Abstractions.Extensions;
using ITMO.Dev.ASAP.WebUI.Abstractions.SafeExecution;
using Microsoft.Extensions.Logging;

namespace ITMO.Dev.ASAP.WebUI.Application.ViewModels.Structure.SubjectCourses.Assignments;

public class SubjectCourseAssignmentList : ISubjectCourseAssignmentList, IDisposable
{
    private readonly ISafeExecutor _safeExecutor;
    private readonly IMessageProducer _producer;
    private readonly IMessageConsumer _consumer;
    private readonly ILogger<SubjectCourseAssignmentList> _logger;
    private readonly ISubjectCourseClient _subjectCourseClient;

    private readonly List<ISubjectCourseAssignmentRow> _rows;
    private readonly IDisposable _subscription;

    private Guid? _subjectCourseId;
    private bool _isSelected;

    public SubjectCourseAssignmentList(
        IMessageProducer producer,
        IMessageConsumer consumer,
        ISafeExecutor safeExecutor,
        ILogger<SubjectCourseAssignmentList> logger,
        ISubjectCourseClient subjectCourseClient)
    {
        _producer = producer;
        _consumer = consumer;
        _safeExecutor = safeExecutor;
        _logger = logger;
        _subjectCourseClient = subjectCourseClient;

        _rows = new List<ISubjectCourseAssignmentRow>();

        _subscription = new SubscriptionBuilder()
            .Subscribe(producer.Observe<SubjectCourseSelectedEvent>().Subscribe(OnSubjectCourseSelected))
            .Subscribe(producer.Observe<SubjectCourseSelectionUpdatedEvent>().Subscribe(OnSelectionUpdated))
            .Subscribe(producer.Observe<AssignmentCreatedEvent>().Subscribe(OnAssignmentCreated))
            .Build();

        Assignments = producer.Observe<SubjectCourseAssignmentListUpdatedEvent>();
    }

    public IObservable<SubjectCourseAssignmentListUpdatedEvent> Assignments { get; }

    public void Dispose()
    {
        _subscription.Dispose();
    }

    private async void OnSubjectCourseSelected(SubjectCourseSelectedEvent evt)
    {
        _subjectCourseId = evt.SubjectCourseId;
        await ReloadAssignments();
    }

    private async void OnSelectionUpdated(SubjectCourseSelectionUpdatedEvent evt)
    {
        _isSelected = evt.Selection is SubjectCourseSelection.Assignments;
        await ReloadAssignments();
    }

    private void OnAssignmentCreated(AssignmentCreatedEvent evt)
    {
        var assignment = new SubjectCourseAssignmentRow(evt.Assignment, _producer, _consumer);
        _rows.Add(assignment);
    }

    private async Task ReloadAssignments()
    {
        if (_isSelected is false)
        {
            return;
        }

        if (_subjectCourseId is null)
        {
            _logger.LogWarning("No subject course selected");
            return;
        }

        await using ISafeExecutionBuilder<IReadOnlyCollection<AssignmentDto>> builder = _safeExecutor
            .Execute(() => _subjectCourseClient.GetAssignmentsAsync(_subjectCourseId.Value));

        builder.Title = "Failed to load subject course assignments";

        builder.OnSuccess(assignments =>
        {
            IEnumerable<Guid> existingIds = _rows.Select(x => x.Id);

            IEnumerable<AssignmentUpdatedEvent> assignmentsToUpdate = assignments
                .IntersectBy(existingIds, x => x.Id)
                .Select(x => new AssignmentUpdatedEvent(x));

            IEnumerable<SubjectCourseAssignmentRow> assignmentsToAdd = assignments
                .ExceptBy(existingIds, x => x.Id)
                .Select(x => new SubjectCourseAssignmentRow(x, _producer, _consumer));

            var assignmentIds = assignments.Select(x => x.Id).ToHashSet();

            _consumer.SendRange(assignmentsToUpdate);
            _rows.AddRange(assignmentsToAdd);
            _rows.RemoveAll(x => assignmentIds.Contains(x.Id) is false);

            var evt = new SubjectCourseAssignmentListUpdatedEvent(_rows);
            _consumer.Send(evt);
        });
    }
}