using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Structure.Subjects;

namespace ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Events.Subjects;

public record SubjectListUpdatedEvent(IEnumerable<ISubjectRowViewModel> Subjects);