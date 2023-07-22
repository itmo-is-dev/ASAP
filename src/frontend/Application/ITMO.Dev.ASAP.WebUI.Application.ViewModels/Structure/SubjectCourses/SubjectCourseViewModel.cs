using ITMO.Dev.ASAP.Application.Dto.Study;
using ITMO.Dev.ASAP.Application.Dto.SubjectCourses;
using ITMO.Dev.ASAP.WebApi.Abstractions.Models;
using ITMO.Dev.ASAP.WebApi.Sdk.ControllerClients;
using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Events.Assignments;
using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Events.SubjectCourses;
using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Messaging;
using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Models;
using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Structure.SubjectCourses;
using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Tools;
using ITMO.Dev.ASAP.WebUI.Abstractions.ExceptionHandling;
using ITMO.Dev.ASAP.WebUI.Abstractions.Extensions;
using ITMO.Dev.ASAP.WebUI.Abstractions.SafeExecution;
using Microsoft.Extensions.Logging;
using System.Reactive.Linq;

namespace ITMO.Dev.ASAP.WebUI.Application.ViewModels.Structure.SubjectCourses;

public class SubjectCourseViewModel : ISubjectCourse, IDisposable
{
    private readonly IMessagePublisher _publisher;
    private readonly ISafeExecutor _safeExecutor;
    private readonly IAssignmentClient _assignmentClient;
    private readonly ISubjectCourseClient _subjectCourseClient;
    private readonly ILogger<SubjectCourseViewModel> _logger;
    private readonly IDisposable _subscription;

    private Guid? _subjectCourseId;

    public SubjectCourseViewModel(
        IMessagePublisher publisher,
        IMessageProvider provider,
        ISafeExecutor safeExecutor,
        IAssignmentClient assignmentClient,
        ILogger<SubjectCourseViewModel> logger,
        ISubjectCourseClient subjectCourseClient)
    {
        _publisher = publisher;
        _safeExecutor = safeExecutor;
        _assignmentClient = assignmentClient;
        _logger = logger;
        _subjectCourseClient = subjectCourseClient;

        SubjectCourse = provider
            .Observe<CurrentSubjectCourseLoadedEvent>()
            .Select(x => x.SubjectCourse);

        Selection = provider
            .Observe<SubjectCourseSelectionUpdatedEvent>()
            .Select(x => x.Selection);

        _subscription = new SubscriptionBuilder()
            .Subscribe(SubjectCourse.Subscribe(x => _subjectCourseId = x.Id))
            .Build();
    }

    public IObservable<SubjectCourseDto> SubjectCourse { get; }

    public IObservable<SubjectCourseSelection> Selection { get; }

    public async ValueTask SelectSubjectCourseAsync(Guid subjectCourseId)
    {
        var selectedEvent = new SubjectCourseSelectedEvent(subjectCourseId);
        _publisher.Send(selectedEvent);

        await using ISafeExecutionBuilder<SubjectCourseDto> builder = _safeExecutor
            .Execute(() => _subjectCourseClient.GetAsync(subjectCourseId));

        builder.Title = "Failed to load current subject course";

        builder.OnSuccess(subjectCourse =>
        {
            var loadedEvent = new CurrentSubjectCourseLoadedEvent(subjectCourse);
            _publisher.Send(loadedEvent);
        });
    }

    public void SelectTab(SubjectCourseSelection selection)
    {
        var evt = new SubjectCourseSelectionUpdatedEvent(selection);
        _publisher.Send(evt);
    }

    public async ValueTask CreateAssignmentAsync(
        string title,
        string shortName,
        int order,
        double minPoints,
        double maxPoints,
        CancellationToken cancellationToken)
    {
        if (_subjectCourseId is null)
        {
            _logger.LogWarning("No subject course was selected");
            return;
        }

        await using ISafeExecutionBuilder<AssignmentDto> builder = _safeExecutor.Execute(() =>
        {
            var request = new CreateAssignmentRequest(
                _subjectCourseId.Value,
                title,
                shortName,
                order,
                minPoints,
                maxPoints);

            return _assignmentClient.CreateAssignmentAsync(request, cancellationToken);
        });

        builder.Title = "Failed to create assignment";

        builder.OnSuccess(assignment =>
        {
            var evt = new AssignmentCreatedEvent(assignment);
            _publisher.Send(evt);
        });
    }

    public void Dispose()
    {
        _subscription.Dispose();
    }
}