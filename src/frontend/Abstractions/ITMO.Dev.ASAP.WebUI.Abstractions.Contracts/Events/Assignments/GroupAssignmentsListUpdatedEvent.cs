using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Structure.Assignments;

namespace ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Events.Assignments;

public record GroupAssignmentsListUpdatedEvent(IEnumerable<IGroupAssignment> GroupAssignments);