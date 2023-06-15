using ITMO.Dev.ASAP.DataAccess.Models;
using ITMO.Dev.ASAP.Domain.Groups;
using ITMO.Dev.ASAP.Domain.Study.Assignments;
using ITMO.Dev.ASAP.Domain.Study.GroupAssignments;

namespace ITMO.Dev.ASAP.DataAccess.Mapping;

public static class GroupAssignmentMapper
{
    public static GroupAssignment MapTo(
        GroupAssignmentModel model,
        string groupName,
        string assignmentTitle,
        string assignmentShortName)
    {
        return new GroupAssignment(
            new GroupAssignmentId(model.StudentGroupId, model.AssignmentId),
            model.Deadline,
            new StudentGroupInfo(model.StudentGroupId, groupName),
            new AssignmentInfo(model.AssignmentId, assignmentTitle, assignmentShortName));
    }

    public static GroupAssignmentModel MapFrom(GroupAssignment entity)
    {
        return new GroupAssignmentModel(entity.Id.StudentGroupId, entity.Id.AssignmentId, entity.Deadline);
    }
}