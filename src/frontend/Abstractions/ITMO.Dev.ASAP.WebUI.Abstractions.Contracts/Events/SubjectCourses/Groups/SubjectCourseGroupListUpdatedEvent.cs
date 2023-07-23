using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Structure.SubjectCourses.Groups;

namespace ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Events.SubjectCourses.Groups;

public record SubjectCourseGroupListUpdatedEvent(IEnumerable<ISubjectCourseGroupRow> SubjectCourseGroups);