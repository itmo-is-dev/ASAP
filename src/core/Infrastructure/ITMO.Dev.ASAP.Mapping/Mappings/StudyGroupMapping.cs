using ITMO.Dev.ASAP.Application.Dto.Study;
using ITMO.Dev.ASAP.Domain.Groups;

namespace ITMO.Dev.ASAP.Mapping.Mappings;

public static class StudyGroupMapping
{
    public static StudyGroupDto ToDto(this StudentGroup group)
    {
        return new StudyGroupDto(group.Id, group.Name);
    }
}