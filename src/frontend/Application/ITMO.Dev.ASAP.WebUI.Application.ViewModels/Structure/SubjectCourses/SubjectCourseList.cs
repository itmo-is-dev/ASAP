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

namespace ITMO.Dev.ASAP.WebUI.Application.ViewModels.Structure.SubjectCourses;

public class SubjectCourseList : ISubjectCourseList, IDisposable
{
    private readonly List<ISubjectCourseRow> _viewModels;
    private readonly ISubjectClient _subjectClient;
    private readonly IMessageConsumer _consumer;
    private readonly IMessageProducer _producer;
    private readonly ISafeExecutor _safeExecutor;
    private readonly IDisposable _subscription;

    public SubjectCourseList(
        ISubjectClient subjectClient,
        IMessageConsumer consumer,
        IMessageProducer producer,
        ISafeExecutor safeExecutor)
    {
        _consumer = consumer;
        _producer = producer;
        _safeExecutor = safeExecutor;
        _subjectClient = subjectClient;

        _viewModels = new List<ISubjectCourseRow>();

        _subscription = new SubscriptionBuilder()
            .Subscribe(producer.Observe<SubjectCourseCreatedEvent>().Subscribe(OnSubjectCourseCreated))
            .Subscribe(producer.Observe<SubjectSelectedEvent>().Subscribe(OnSubjectSelected))
            .Subscribe(producer.Observe<NavigatedToGlobalPageEvent>().Subscribe(_ => ClearSelection()))
            .Subscribe(producer.Observe<NavigatedToGroupsPageEvent>().Subscribe(_ => ClearSelection()))
            .Subscribe(producer.Observe<NavigatedToSettingsPageEvent>().Subscribe(_ => ClearSelection()))
            .Subscribe(producer.Observe<NavigatedToStudentsPageEvent>().Subscribe(_ => ClearSelection()))
            .Subscribe(producer.Observe<NavigatedToSubjectsPageEvent>().Subscribe(_ => ClearSelection()))
            .Subscribe(producer.Observe<NavigatedToUsersPageEvent>().Subscribe(_ => ClearSelection()))
            .Build();
    }

    public IObservable<SubjectCourseListUpdatedEvent> SubjectCourses
        => _producer.Observe<SubjectCourseListUpdatedEvent>();

    public void Dispose()
    {
        _subscription.Dispose();
    }

    private void OnSubjectCourseCreated(SubjectCourseCreatedEvent evt)
    {
        var viewModel = new SubjectCourseRow(evt.SubjectCourse, _consumer, _producer);
        _viewModels.Add(viewModel);

        var updatedEvent = new SubjectCourseListUpdatedEvent(_viewModels);
        _consumer.Send(updatedEvent);
    }

    private async void OnSubjectSelected(SubjectSelectedEvent evt)
    {
        if (evt.SubjectId is null)
        {
            _viewModels.Clear();

            var coursesUpdatedEvent = new SubjectCourseListUpdatedEvent(_viewModels);
            _consumer.Send(coursesUpdatedEvent);

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
                .Select(x => new SubjectCourseRow(x, _consumer, _producer));

            var courseIds = courses.Select(x => x.Id).ToHashSet();

            _consumer.SendRange(coursesToUpdate);
            _viewModels.AddRange(coursesToAdd);
            _viewModels.RemoveAll(x => courseIds.Contains(x.Id) is false);

            var coursesUpdatedEvent = new SubjectCourseListUpdatedEvent(_viewModels);
            _consumer.Send(coursesUpdatedEvent);
        });
    }

    private void ClearSelection()
    {
        var evt = new SubjectCourseSelectedEvent(null);
        _consumer.Send(evt);
    }
}