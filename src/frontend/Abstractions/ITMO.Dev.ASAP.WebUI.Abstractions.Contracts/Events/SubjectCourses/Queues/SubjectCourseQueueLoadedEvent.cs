using ITMO.Dev.ASAP.Application.Dto.Tables;

namespace ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Events.SubjectCourses.Queues;

public record SubjectCourseQueueLoadedEvent(SubmissionsQueueDto Queue);