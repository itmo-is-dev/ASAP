using ITMO.Dev.ASAP.Application.Dto.SubjectCourses;

namespace ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Events.SubjectCourses;

public record SubjectCourseUpdatedEvent(SubjectCourseDto SubjectCourse);