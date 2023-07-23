using ITMO.Dev.ASAP.DataAccess.Models;
using ITMO.Dev.ASAP.Domain.Groups;

namespace ITMO.Dev.ASAP.DataAccess.Mapping;

public static class StudentGroupMapper
{
    public static StudentGroup MapTo(StudentGroupModel model, HashSet<Guid> studentIds)
    {
        return new StudentGroup(model.Id, model.Name, studentIds);
    }

    public static StudentGroupModel MapFrom(StudentGroup entity)
    {
        return new StudentGroupModel(entity.Id, entity.Name);
    }
}