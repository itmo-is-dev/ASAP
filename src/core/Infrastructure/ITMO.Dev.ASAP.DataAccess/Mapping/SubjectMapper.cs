using ITMO.Dev.ASAP.DataAccess.Models;
using ITMO.Dev.ASAP.Domain.Study;

namespace ITMO.Dev.ASAP.DataAccess.Mapping;

public static class SubjectMapper
{
    public static Subject MapTo(SubjectModel model)
    {
        return new Subject(model.Id, model.Title);
    }

    public static SubjectModel MapFrom(Subject entity)
    {
        return new SubjectModel(entity.Id, entity.Title);
    }
}