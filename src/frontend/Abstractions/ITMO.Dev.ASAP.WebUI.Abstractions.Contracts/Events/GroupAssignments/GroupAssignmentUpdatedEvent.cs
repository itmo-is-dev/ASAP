using ITMO.Dev.ASAP.Application.Dto.Study;

namespace ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Events.GroupAssignments;

public record GroupAssignmentUpdatedEvent(GroupAssignmentDto GroupAssignment);