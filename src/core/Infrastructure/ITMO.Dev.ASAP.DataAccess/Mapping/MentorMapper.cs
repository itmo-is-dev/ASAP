using ITMO.Dev.ASAP.DataAccess.Models.Users;
using ITMO.Dev.ASAP.Domain.Users;

namespace ITMO.Dev.ASAP.DataAccess.Mapping;

public static class MentorMapper
{
    public static Mentor MapTo(MentorModel model)
    {
        return new Mentor(model.UserId, model.SubjectCourseId);
    }

    public static MentorModel MapFrom(Mentor entity)
    {
        return new MentorModel(entity.UserId, entity.SubjectCourseId);
    }
}