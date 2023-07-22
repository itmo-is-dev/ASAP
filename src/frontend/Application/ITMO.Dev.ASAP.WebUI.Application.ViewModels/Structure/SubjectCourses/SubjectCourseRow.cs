using ITMO.Dev.ASAP.Application.Dto.SubjectCourses;
using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Events.SubjectCourses;
using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Messaging;
using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Structure.SubjectCourses;
using System.Reactive.Linq;

namespace ITMO.Dev.ASAP.WebUI.Application.ViewModels.Structure.SubjectCourses;

public class SubjectCourseRow : ISubjectCourseRow
{
    public SubjectCourseRow(
        SubjectCourseDto subjectCourse,
        IMessagePublisher publisher,
        IMessageProvider provider)
    {
        Id = subjectCourse.Id;

        Title = provider.Observe<SubjectCourseUpdatedEvent>()
            .Where(x => x.SubjectCourse.Id.Equals(subjectCourse.Id))
            .Select(x => x.SubjectCourse.Title)
            .Prepend(subjectCourse.Title)
            .Replay(1)
            .AutoConnect();

        IsSelected = provider.Observe<SubjectCourseSelectedEvent>()
            .Select(x => x.SubjectCourseId.Equals(subjectCourse.Id))
            .Prepend(false)
            .Replay(1)
            .AutoConnect();
    }

    public Guid Id { get; }

    public IObservable<string> Title { get; }

    public IObservable<bool> IsSelected { get; }
}