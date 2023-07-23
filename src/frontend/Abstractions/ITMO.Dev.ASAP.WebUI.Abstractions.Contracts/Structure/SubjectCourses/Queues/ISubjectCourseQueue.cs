using ITMO.Dev.ASAP.Application.Dto.Tables;

namespace ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Structure.SubjectCourses.Queues;

public interface ISubjectCourseQueue
{
    IObservable<SubmissionsQueueDto> Queue { get; }

    ValueTask UnsubscribeAsync(CancellationToken cancellationToken);
}