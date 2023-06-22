using ITMO.Dev.ASAP.Application.Dto.Study;

namespace ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Events.Subjects;

public record SubjectUpdatedEvent(SubjectDto Subject);