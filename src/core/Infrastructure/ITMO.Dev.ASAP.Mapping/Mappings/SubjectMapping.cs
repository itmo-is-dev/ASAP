using ITMO.Dev.ASAP.Application.Dto.Study;
using Subject = ITMO.Dev.ASAP.Domain.Study.Subject;

namespace ITMO.Dev.ASAP.Mapping.Mappings;

public static class SubjectMapping
{
    public static SubjectDto ToDto(this Subject subject)
    {
        return new SubjectDto(subject.Id, subject.Title);
    }
}