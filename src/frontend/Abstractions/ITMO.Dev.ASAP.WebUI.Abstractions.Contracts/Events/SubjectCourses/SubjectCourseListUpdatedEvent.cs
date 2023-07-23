using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Structure.SubjectCourses;

namespace ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Events.SubjectCourses;

public record SubjectCourseListUpdatedEvent(IEnumerable<ISubjectCourseRow> SubjectCourses);