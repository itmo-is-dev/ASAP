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
using System.Reactive.Linq;

namespace ITMO.Dev.ASAP.WebUI.Application.ViewModels.Structure.SubjectCourses.Assignments;

public class SubjectCourseAssignmentList : ISubjectCourseAssignmentList, IDisposable
{
    private readonly ISafeExecutor _safeExecutor;
    private readonly IMessageProvider _provider;
    private readonly IMessagePublisher _publisher;
    private readonly ILogger<SubjectCourseAssignmentList> _logger;
    private readonly ISubjectCourseClient _subjectCourseClient;

    private readonly List<ISubjectCourseAssignmentRow> _rows;
    private readonly IDisposable _subscription;

    private Guid? _subjectCourseId;
    private bool _isSelected;

    public SubjectCourseAssignmentList(
        IMessageProvider provider,
        IMessagePublisher publisher,
        ISafeExecutor safeExecutor,
        ILogger<SubjectCourseAssignmentList> logger,
        ISubjectCourseClient subjectCourseClient)
    {
        _provider = provider;
        _publisher = publisher;
        _safeExecutor = safeExecutor;
        _logger = logger;
        _subjectCourseClient = subjectCourseClient;

        _rows = new List<ISubjectCourseAssignmentRow>();

        _subscription = new SubscriptionBuilder()
            .Subscribe(provider.Observe<SubjectCourseSelectedEvent>().Subscribe(OnSubjectCourseSelected))
            .Subscribe(provider.Observe<SubjectCourseSelectionUpdatedEvent>().Subscribe(OnSelectionUpdated))
            .Subscribe(provider.Observe<AssignmentCreatedEvent>().Subscribe(OnAssignmentCreated))
            .Build();

        Assignments = provider
            .Observe<SubjectCourseAssignmentListUpdatedEvent>()
            .Select(x => x.Assignments);
    }

    public IObservable<IEnumerable<ISubjectCourseAssignmentRow>> Assignments { get; }

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
        var assignment = new SubjectCourseAssignmentRow(evt.Assignment, _provider, _publisher);
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
                .Select(x => new SubjectCourseAssignmentRow(x, _provider, _publisher));

            var assignmentIds = assignments.Select(x => x.Id).ToHashSet();

            _publisher.SendRange(assignmentsToUpdate);
            _rows.AddRange(assignmentsToAdd);
            _rows.RemoveAll(x => assignmentIds.Contains(x.Id) is false);

            var evt = new SubjectCourseAssignmentListUpdatedEvent(_rows);
            _publisher.Send(evt);
        });
    }
}