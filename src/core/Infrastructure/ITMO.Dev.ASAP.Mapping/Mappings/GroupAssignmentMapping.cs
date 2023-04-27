using ITMO.Dev.ASAP.Application.Dto.Study;
using GroupAssignment = ITMO.Dev.ASAP.Domain.Study.GroupAssignment;

namespace ITMO.Dev.ASAP.Mapping.Mappings;

public static class GroupAssignmentMapping
{
    public static GroupAssignmentDto ToDto(this GroupAssignment groupAssignment)
    {
        return new GroupAssignmentDto(
            groupAssignment.GroupId,
            groupAssignment.Group.Name,
            groupAssignment.AssignmentId,
            groupAssignment.Assignment.Title,
            groupAssignment.Deadline.AsDateTime());
    }
}