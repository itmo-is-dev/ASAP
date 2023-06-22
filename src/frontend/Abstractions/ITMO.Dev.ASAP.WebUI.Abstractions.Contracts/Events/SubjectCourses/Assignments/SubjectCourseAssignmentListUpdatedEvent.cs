using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Structure.SubjectCourses.Assignments;

namespace ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Events.SubjectCourses.Assignments;

public record SubjectCourseAssignmentListUpdatedEvent(IEnumerable<ISubjectCourseAssignmentRow> Assignments);