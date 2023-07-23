using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Structure.SubjectCourses.Queues;

namespace ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Events.SubjectCourses.Queues;

public record SubjectCourseQueueListUpdatedEvent(IEnumerable<ISubjectCourseQueueRow> Rows);