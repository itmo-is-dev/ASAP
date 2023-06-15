using ITMO.Dev.ASAP.Application.Dto.Study;
using ITMO.Dev.ASAP.Domain.Study.GroupAssignments;

namespace ITMO.Dev.ASAP.Mapping.Mappings;

public static class GroupAssignmentMapping
{
    public static GroupAssignmentDto ToDto(this GroupAssignment groupAssignment)
    {
        return new GroupAssignmentDto(
            groupAssignment.Id.StudentGroupId,
            groupAssignment.Group.Name,
            groupAssignment.Id.AssignmentId,
            groupAssignment.Assignment.Title,
            groupAssignment.Deadline.AsDateTime());
    }
}