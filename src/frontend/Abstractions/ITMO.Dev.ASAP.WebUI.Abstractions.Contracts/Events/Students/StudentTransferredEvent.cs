using ITMO.Dev.ASAP.Application.Dto.Users;

namespace ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Events.Students;

public record StudentTransferredEvent(StudentDto Student);