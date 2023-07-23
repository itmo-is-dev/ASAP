using ITMO.Dev.ASAP.Application.Dto.SubjectCourses;

namespace ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Events.SubjectCourses.Groups;

public record SubjectCourseGroupUpdatedEvent(SubjectCourseGroupDto Group);