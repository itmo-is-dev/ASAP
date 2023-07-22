using ITMO.Dev.ASAP.Application.Dto.SubjectCourses;
using ITMO.Dev.ASAP.WebApi.Abstractions.Models.SubjectCourseGroups;
using ITMO.Dev.ASAP.WebApi.Sdk.ControllerClients;
using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Events.SubjectCourses;
using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Events.SubjectCourses.Groups;
using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Messaging;
using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Models;
using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Structure.SubjectCourses.Groups;
using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Tools;
using ITMO.Dev.ASAP.WebUI.Abstractions.ExceptionHandling;
using ITMO.Dev.ASAP.WebUI.Abstractions.Extensions;
using ITMO.Dev.ASAP.WebUI.Abstractions.SafeExecution;
using Microsoft.Extensions.Logging;
using System.Reactive.Linq;

namespace ITMO.Dev.ASAP.WebUI.Application.ViewModels.Structure.SubjectCourses.Groups;

public class SubjectCourseGroupList : ISubjectCourseGroupList, IDisposable
{
    private readonly IMessageProvider _provider;
    private readonly IMessagePublisher _publisher;
    private readonly ILogger<SubjectCourseGroupList> _logger;
    private readonly ISafeExecutor _safeExecutor;
    private readonly ISubjectCourseClient _subjectCourseClient;
    private readonly ISubjectCourseGroupClient _subjectCourseGroupClient;

    private readonly IDisposable _subscription;
    private readonly List<ISubjectCourseGroupRow> _rows;

    private Guid? _subjectCourseId;
    private bool _isSelected;

    public SubjectCourseGroupList(
        IMessageProvider provider,
        IMessagePublisher publisher,
        ILogger<SubjectCourseGroupList> logger,
        ISafeExecutor safeExecutor,
        ISubjectCourseClient subjectCourseClient,
        ISubjectCourseGroupClient subjectCourseGroupClient)
    {
        _provider = provider;
        _publisher = publisher;
        _logger = logger;
        _safeExecutor = safeExecutor;
        _subjectCourseClient = subjectCourseClient;
        _subjectCourseGroupClient = subjectCourseGroupClient;

        _subscription = new SubscriptionBuilder()
            .Subscribe(provider.Observe<SubjectCourseSelectedEvent>().Subscribe(OnSubjectCourseSelected))
            .Subscribe(provider.Observe<SubjectCourseSelectionUpdatedEvent>().Subscribe(OnSelectionChanged))
            .Subscribe(provider.Observe<AddSubjectCourseGroupsVisibleEvent>()
                .Subscribe(OnAddSubjectCourseGroupsVisibleEvent))
            .Build();

        _rows = new List<ISubjectCourseGroupRow>();

        SubjectCourseGroups = provider
            .Observe<SubjectCourseGroupListUpdatedEvent>()
            .Select(x => x.SubjectCourseGroups);

        AddSubjectCourseGroupsVisible = provider
            .Observe<AddSubjectCourseGroupsVisibleEvent>()
            .Select(x => x.IsVisible);
    }

    public IObservable<IEnumerable<ISubjectCourseGroupRow>> SubjectCourseGroups { get; }

    public IObservable<bool> AddSubjectCourseGroupsVisible { get; }

    public void ShowAddSubjectCourseGroups()
    {
        var evt = new AddSubjectCourseGroupsVisibleEvent(true);
        _publisher.Send(evt);
    }

    public async ValueTask AddAsync(IReadOnlyCollection<Guid> studentGroupIds, CancellationToken cancellationToken)
    {
        if (_subjectCourseId is null)
        {
            _logger.LogWarning("No subject course selected");
            return;
        }

        await using ISafeExecutionBuilder<IReadOnlyCollection<SubjectCourseGroupDto>> builder = _safeExecutor.Execute(() =>
        {
            var request = new BulkCreateSubjectCourseGroupsRequest(_subjectCourseId.Value, studentGroupIds);
            return _subjectCourseGroupClient.BulkCreateAsync(request, cancellationToken);
        });

        builder.Title = "Failed to add student groups to subject course";

        builder.OnSuccess(subjectCourseGroups =>
        {
            IEnumerable<SubjectCourseGroupRow> rows = subjectCourseGroups
                .Select(x => new SubjectCourseGroupRow(x, _provider, _publisher));

            _rows.AddRange(rows);

            var evt = new SubjectCourseGroupListUpdatedEvent(_rows);
            _publisher.Send(evt);
        });
    }

    public void Dispose()
    {
        _subscription.Dispose();
    }

    private async void OnSubjectCourseSelected(SubjectCourseSelectedEvent evt)
    {
        _subjectCourseId = evt.SubjectCourseId;

        if (_isSelected)
        {
            await ReloadSubjectCourseGroups();
        }
    }

    private async void OnSelectionChanged(SubjectCourseSelectionUpdatedEvent evt)
    {
        _isSelected = evt.Selection is SubjectCourseSelection.Groups;

        if (_isSelected)
        {
            await ReloadSubjectCourseGroups();
        }
    }

    private async void OnAddSubjectCourseGroupsVisibleEvent(AddSubjectCourseGroupsVisibleEvent evt)
    {
        if (evt.IsVisible)
        {
            await ReloadSubjectCourseGroups();
        }
    }

    private async ValueTask ReloadSubjectCourseGroups()
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

            IEnumerable<SubjectCourseGroupRow> assignmentsToAdd = assignments
                .ExceptBy(existingIds, x => (x.SubjectCourseId, x.StudentGroupId))
                .Select(x => new SubjectCourseGroupRow(x, _provider, _publisher));

            var assignmentIds = assignments
                .Select(x => (x.SubjectCourseId, x.StudentGroupId))
                .ToHashSet();

            _publisher.SendRange(assignmentsToUpdate);
            _rows.AddRange(assignmentsToAdd);
            _rows.RemoveAll(x => assignmentIds.Contains((x.SubjectCourseId, x.StudentGroupId)) is false);

            var evt = new SubjectCourseGroupListUpdatedEvent(_rows);
            _publisher.Send(evt);
        });
    }
}