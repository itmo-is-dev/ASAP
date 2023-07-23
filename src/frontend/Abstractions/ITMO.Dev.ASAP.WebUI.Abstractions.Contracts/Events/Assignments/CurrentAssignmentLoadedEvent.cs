using ITMO.Dev.ASAP.Application.Dto.Study;

namespace ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Events.Assignments;

public record CurrentAssignmentLoadedEvent(AssignmentDto Assignment);