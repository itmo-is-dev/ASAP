using ITMO.Dev.ASAP.Application.Dto.SubjectCourses;
using ITMO.Dev.ASAP.WebApi.Sdk.ControllerClients;
using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Events.Navigation;
using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Events.SubjectCourses;
using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Events.Subjects;
using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Messaging;
using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Structure.SubjectCourses;
using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Tools;
using ITMO.Dev.ASAP.WebUI.Abstractions.ExceptionHandling;
using ITMO.Dev.ASAP.WebUI.Abstractions.Extensions;
using ITMO.Dev.ASAP.WebUI.Abstractions.SafeExecution;
using System.Reactive.Linq;

namespace ITMO.Dev.ASAP.WebUI.Application.ViewModels.Structure.SubjectCourses;

public class SubjectCourseList : ISubjectCourseList, IDisposable
{
    private readonly List<ISubjectCourseRow> _viewModels;
    private readonly ISubjectClient _subjectClient;
    private readonly IMessagePublisher _publisher;
    private readonly IMessageProvider _provider;
    private readonly ISafeExecutor _safeExecutor;
    private readonly IDisposable _subscription;

    public SubjectCourseList(
        ISubjectClient subjectClient,
        IMessagePublisher publisher,
        IMessageProvider provider,
        ISafeExecutor safeExecutor)
    {
        _publisher = publisher;
        _provider = provider;
        _safeExecutor = safeExecutor;
        _subjectClient = subjectClient;

        _viewModels = new List<ISubjectCourseRow>();

        _subscription = new SubscriptionBuilder()
            .Subscribe(provider.Observe<SubjectCourseCreatedEvent>().Subscribe(OnSubjectCourseCreated))
            .Subscribe(provider.Observe<SubjectSelectedEvent>().Subscribe(OnSubjectSelected))
            .Subscribe(provider.Observe<NavigatedToGlobalPageEvent>().Subscribe(_ => ClearSelection()))
            .Subscribe(provider.Observe<NavigatedToGroupsPageEvent>().Subscribe(_ => ClearSelection()))
            .Subscribe(provider.Observe<NavigatedToSettingsPageEvent>().Subscribe(_ => ClearSelection()))
            .Subscribe(provider.Observe<NavigatedToStudentsPageEvent>().Subscribe(_ => ClearSelection()))
            .Subscribe(provider.Observe<NavigatedToSubjectsPageEvent>().Subscribe(_ => ClearSelection()))
            .Subscribe(provider.Observe<NavigatedToUsersPageEvent>().Subscribe(_ => ClearSelection()))
            .Build();

        SubjectCourses = _provider
            .Observe<SubjectCourseListUpdatedEvent>()
            .Select(x => x.SubjectCourses);
    }

    public IObservable<IEnumerable<ISubjectCourseRow>> SubjectCourses { get; }

    public void Dispose()
    {
        _subscription.Dispose();
    }

    private void OnSubjectCourseCreated(SubjectCourseCreatedEvent evt)
    {
        var viewModel = new SubjectCourseRow(evt.SubjectCourse, _publisher, _provider);
        _viewModels.Add(viewModel);

        var updatedEvent = new SubjectCourseListUpdatedEvent(_viewModels);
        _publisher.Send(updatedEvent);
    }

    private async void OnSubjectSelected(SubjectSelectedEvent evt)
    {
        if (evt.SubjectId is null)
        {
            _viewModels.Clear();

            var coursesUpdatedEvent = new SubjectCourseListUpdatedEvent(_viewModels);
            _publisher.Send(coursesUpdatedEvent);

            return;
        }

        await using ISafeExecutionBuilder<IReadOnlyCollection<SubjectCourseDto>> builder = _safeExecutor
            .Execute(() => _subjectClient.GetCoursesAsync(evt.SubjectId.Value));

        builder.Title = "Failed to load subject courses";

        builder.OnSuccess(courses =>
        {
            IEnumerable<Guid> existingIds = _viewModels.Select(x => x.Id);

            IEnumerable<SubjectCourseUpdatedEvent> coursesToUpdate = courses
                .IntersectBy(existingIds, x => x.Id)
                .Select(x => new SubjectCourseUpdatedEvent(x));

            IEnumerable<SubjectCourseRow> coursesToAdd = courses
                .ExceptBy(existingIds, x => x.Id)
                .Select(x => new SubjectCourseRow(x, _publisher, _provider));

            var courseIds = courses.Select(x => x.Id).ToHashSet();

            _publisher.SendRange(coursesToUpdate);
            _viewModels.AddRange(coursesToAdd);
            _viewModels.RemoveAll(x => courseIds.Contains(x.Id) is false);

            var coursesUpdatedEvent = new SubjectCourseListUpdatedEvent(_viewModels);
            _publisher.Send(coursesUpdatedEvent);
        });
    }

    private void ClearSelection()
    {
        var evt = new SubjectCourseSelectedEvent(null);
        _publisher.Send(evt);
    }
}