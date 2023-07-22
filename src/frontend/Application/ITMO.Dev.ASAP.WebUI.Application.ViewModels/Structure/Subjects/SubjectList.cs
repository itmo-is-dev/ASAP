using ITMO.Dev.ASAP.Application.Dto.Study;
using ITMO.Dev.ASAP.WebApi.Sdk.ControllerClients;
using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Events.Navigation;
using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Events.SubjectCourses;
using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Events.Subjects;
using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Messaging;
using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Structure.Subjects;
using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Tools;
using ITMO.Dev.ASAP.WebUI.Abstractions.ExceptionHandling;
using ITMO.Dev.ASAP.WebUI.Abstractions.Extensions;
using ITMO.Dev.ASAP.WebUI.Abstractions.SafeExecution;

namespace ITMO.Dev.ASAP.WebUI.Application.ViewModels.Structure.Subjects;

public class SubjectList : ISubjectList, IDisposable
{
    private readonly List<ISubjectRowViewModel> _subjectViewModels;
    private readonly ISubjectClient _subjectClient;
    private readonly IMessagePublisher _publisher;
    private readonly IMessageProvider _provider;
    private readonly IDisposable _subscription;
    private readonly ISafeExecutor _safeExecutor;

    public SubjectList(
        ISubjectClient subjectClient,
        IMessagePublisher publisher,
        IMessageProvider provider,
        ISafeExecutor safeExecutor)
    {
        _subjectClient = subjectClient;
        _publisher = publisher;
        _provider = provider;
        _safeExecutor = safeExecutor;

        _subjectViewModels = new List<ISubjectRowViewModel>();

        _subscription = new SubscriptionBuilder()
            .Subscribe(provider.Observe<SubjectCreatedEvent>().Subscribe(OnSubjectCreated))
            .Subscribe(provider.Observe<CurrentSubjectCourseLoadedEvent>().Subscribe(OnCurrentSubjectCourseLoaded))
            .Subscribe(provider.Observe<NavigatedToGlobalPageEvent>().Subscribe(_ => ClearSelection()))
            .Subscribe(provider.Observe<NavigatedToGroupsPageEvent>().Subscribe(_ => ClearSelection()))
            .Subscribe(provider.Observe<NavigatedToSettingsPageEvent>().Subscribe(_ => ClearSelection()))
            .Subscribe(provider.Observe<NavigatedToStudentsPageEvent>().Subscribe(_ => ClearSelection()))
            .Subscribe(provider.Observe<NavigatedToSubjectsPageEvent>().Subscribe(_ => ClearSelection()))
            .Subscribe(provider.Observe<NavigatedToUsersPageEvent>().Subscribe(_ => ClearSelection()))
            .Build();
    }

    public IObservable<SubjectListUpdatedEvent> Subjects => _provider.Observe<SubjectListUpdatedEvent>();

    public async ValueTask LoadAsync(CancellationToken cancellationToken)
    {
        await using ISafeExecutionBuilder<IReadOnlyCollection<SubjectDto>> builder = _safeExecutor
            .Execute(() => _subjectClient.GetAsync(cancellationToken));

        builder.Title = "Failed to load subjects";

        builder.OnSuccess(subjects =>
        {
            IEnumerable<Guid> ids = _subjectViewModels.Select(x => x.Id);

            IEnumerable<SubjectUpdatedEvent> subjectsToUpdate = subjects
                .IntersectBy(ids, x => x.Id)
                .Select(x => new SubjectUpdatedEvent(x));

            IEnumerable<ISubjectRowViewModel> subjectsToAdd = subjects
                .ExceptBy(ids, x => x.Id)
                .Select(x => new SubjectRowViewModel(_publisher, _provider, x));

            _publisher.SendRange(subjectsToUpdate);
            _subjectViewModels.AddRange(subjectsToAdd);

            var evt = new SubjectListUpdatedEvent(_subjectViewModels);
            _publisher.Send(evt);
        });
    }

    public void Dispose()
    {
        _subscription.Dispose();
    }

    private void OnSubjectCreated(SubjectCreatedEvent evt)
    {
        var viewModel = new SubjectRowViewModel(_publisher, _provider, evt.Subject);
        _subjectViewModels.Add(viewModel);

        var updatedEvt = new SubjectListUpdatedEvent(_subjectViewModels);
        _publisher.Send(updatedEvt);
    }

    private void OnCurrentSubjectCourseLoaded(CurrentSubjectCourseLoadedEvent arg)
    {
        var evt = new SubjectSelectedEvent(arg.SubjectCourse.SubjectId);
        _publisher.Send(evt);
    }

    private void ClearSelection()
    {
        var evt = new SubjectSelectedEvent(null);
        _publisher.Send(evt);
    }
}