using ITMO.Dev.ASAP.Application.Dto.Study;

namespace ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Events.StudentGroups;

public record StudentGroupUpdatedEvent(StudyGroupDto Group);