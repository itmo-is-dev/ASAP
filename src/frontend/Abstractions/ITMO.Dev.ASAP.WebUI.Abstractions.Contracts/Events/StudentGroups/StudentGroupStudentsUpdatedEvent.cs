using ITMO.Dev.ASAP.Application.Dto.Users;

namespace ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Events.StudentGroups;

public record StudentGroupStudentsUpdatedEvent(IEnumerable<StudentDto> Students);