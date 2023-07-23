using ITMO.Dev.ASAP.Application.Dto.Study;
using ITMO.Dev.ASAP.Application.Dto.Users;
using ITMO.Dev.ASAP.WebApi.Abstractions.Models.StudyGroups;
using ITMO.Dev.ASAP.WebApi.Sdk.ControllerClients;
using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Events.StudentGroups;
using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Events.Students;
using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Messaging;
using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Structure.StudentGroups;
using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Tools;
using ITMO.Dev.ASAP.WebUI.Abstractions.ExceptionHandling;
using ITMO.Dev.ASAP.WebUI.Abstractions.Extensions;
using ITMO.Dev.ASAP.WebUI.Abstractions.SafeExecution;
using System.Reactive.Linq;

namespace ITMO.Dev.ASAP.WebUI.Application.ViewModels.Structure.StudentGroups;

public class StudentGroup : IStudentGroup, IDisposable
{
    private readonly IMessagePublisher _publisher;
    private readonly ISafeExecutor _safeExecutor;
    private readonly IStudyGroupClient _studyGroupClient;

    private readonly IDisposable _subscription;
    private readonly List<StudentDto> _students;

    public StudentGroup(
        IMessagePublisher publisher,
        IMessageProvider provider,
        ISafeExecutor safeExecutor,
        IStudyGroupClient studyGroupClient)
    {
        _publisher = publisher;
        _safeExecutor = safeExecutor;
        _studyGroupClient = studyGroupClient;

        _students = new List<StudentDto>();

        _subscription = Disposable.From(
            provider.Observe<StudentGroupSelectedEvent>().Subscribe(OnStudentGroupSelected),
            provider.Observe<StudentTransferredEvent>().Subscribe(OnStudentTransferred));

        Name = provider
            .Observe<StudentGroupUpdatedEvent>()
            .Where(x => x.Group.Id.Equals(Id))
            .Select(x => x.Group.Name);

        Students = provider
            .Observe<StudentGroupStudentsUpdatedEvent>()
            .Select(x => x.Students);

        IsVisible = provider
            .Observe<StudentGroupSelectedEvent>()
            .Select(_ => true);
    }

    public Guid Id { get; private set; }

    public IObservable<string> Name { get; }

    public IObservable<IEnumerable<StudentDto>> Students { get; }

    public IObservable<bool> IsVisible { get; }

    public async ValueTask UpdateNameAsync(string name, CancellationToken cancellationToken)
    {
        await using ISafeExecutionBuilder<StudyGroupDto> builder = _safeExecutor.Execute(() =>
        {
            var request = new UpdateStudyGroupRequest(name);
            return _studyGroupClient.UpdateAsync(Id, request, cancellationToken);
        });

        builder.Title = "Failed to updated student group";

        builder.OnSuccess(group =>
        {
            var evt = new StudentGroupUpdatedEvent(group);
            _publisher.Send(evt);
        });
    }

    public void Dispose()
    {
        _subscription.Dispose();
    }

    private async void OnStudentGroupSelected(StudentGroupSelectedEvent evt)
    {
        Id = evt.StudentGroupId;

        await Task.WhenAll(LoadStudentGroupAsync(Id), LoadStudentsAsync(Id));
    }

    private void OnStudentTransferred(StudentTransferredEvent evt)
    {
        if (evt.Student.GroupId.Equals(Id))
        {
            _students.Add(evt.Student);
        }
        else
        {
            _students.RemoveAll(x => x.User.Id.Equals(evt.Student.User.Id));
        }

        var updatedEvent = new StudentGroupStudentsUpdatedEvent(_students);
        _publisher.Send(updatedEvent);
    }

    private async Task LoadStudentGroupAsync(Guid id)
    {
        await using ISafeExecutionBuilder<StudyGroupDto> builder = _safeExecutor
            .Execute(() => _studyGroupClient.GetAsync(id));

        builder.Title = "Failed to load selected group";

        builder.OnSuccess(group =>
        {
            var evt = new StudentGroupUpdatedEvent(group);
            _publisher.Send(evt);
        });
    }

    private async Task LoadStudentsAsync(Guid id)
    {
        await using ISafeExecutionBuilder<IReadOnlyCollection<StudentDto>> builder = _safeExecutor
            .Execute(() => _studyGroupClient.GetStudentsAsync(id));

        builder.Title = "Failed to load selected group students";

        builder.OnSuccess(students =>
        {
            _students.Clear();
            _students.AddRange(students);

            var evt = new StudentGroupStudentsUpdatedEvent(_students);
            _publisher.Send(evt);
        });
    }
}