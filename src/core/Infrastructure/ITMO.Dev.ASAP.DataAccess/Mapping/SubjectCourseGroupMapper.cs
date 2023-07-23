using ITMO.Dev.ASAP.DataAccess.Models;
using ITMO.Dev.ASAP.Domain.Groups;
using ITMO.Dev.ASAP.Domain.Study;

namespace ITMO.Dev.ASAP.DataAccess.Mapping;

public static class SubjectCourseGroupMapper
{
    public static SubjectCourseGroup MapTo(SubjectCourseGroupModel model)
    {
        var groupInfo = new StudentGroupInfo(model.StudentGroupId, model.StudentGroup.Name);
        return new SubjectCourseGroup(model.SubjectCourseId, groupInfo);
    }

    public static SubjectCourseGroupModel MapFrom(SubjectCourseGroup entity)
    {
        return new SubjectCourseGroupModel(
            subjectCourseId: entity.SubjectCourseId,
            studentGroupId: entity.StudentGroupId);
    }
}