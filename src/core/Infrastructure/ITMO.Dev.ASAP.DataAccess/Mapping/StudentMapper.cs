using ITMO.Dev.ASAP.DataAccess.Models.Users;
using ITMO.Dev.ASAP.Domain.Groups;
using ITMO.Dev.ASAP.Domain.Students;
using ITMO.Dev.ASAP.Domain.Users;

namespace ITMO.Dev.ASAP.DataAccess.Mapping;

public static class StudentMapper
{
    public static Student MapTo(StudentModel model)
    {
        User user = UserMapper.MapTo(model.User);

        StudentGroupInfo? group = model.StudentGroup is null
            ? null
            : new StudentGroupInfo(model.StudentGroup.Id, model.StudentGroup.Name);

        return new Student(user, group);
    }

    public static StudentModel MapFrom(Student entity)
    {
        return new StudentModel(entity.UserId, entity.Group?.Id);
    }
}