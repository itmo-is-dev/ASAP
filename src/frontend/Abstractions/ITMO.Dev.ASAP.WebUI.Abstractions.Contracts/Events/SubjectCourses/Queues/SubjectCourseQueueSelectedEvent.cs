namespace ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Events.SubjectCourses.Queues;

public record SubjectCourseQueueSelectedEvent(Guid SubjectCourseId, Guid StudentGroupId);