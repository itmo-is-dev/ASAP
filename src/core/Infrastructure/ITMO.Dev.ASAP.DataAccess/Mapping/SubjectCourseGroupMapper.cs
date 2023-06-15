using ITMO.Dev.ASAP.DataAccess.Models;
using ITMO.Dev.ASAP.Domain.Study;

namespace ITMO.Dev.ASAP.DataAccess.Mapping;

public static class SubjectCourseGroupMapper
{
    public static SubjectCourseGroup MapTo(SubjectCourseGroupModel model)
    {
        return new SubjectCourseGroup(model.SubjectCourseId, model.StudentGroupId);
    }

    public static SubjectCourseGroupModel MapFrom(SubjectCourseGroup entity)
    {
        return new SubjectCourseGroupModel(
            subjectCourseId: entity.SubjectCourseId,
            studentGroupId: entity.StudentGroupId);
    }
}