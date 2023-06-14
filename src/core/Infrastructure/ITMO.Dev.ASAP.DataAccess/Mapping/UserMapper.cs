using ITMO.Dev.ASAP.DataAccess.Models.Users;
using ITMO.Dev.ASAP.Domain.Users;

namespace ITMO.Dev.ASAP.DataAccess.Mapping;

public static class UserMapper
{
    public static User MapTo(UserModel model)
    {
        var associations = model.Associations
            .Select(UserAssociationsMapper.MapTo)
            .ToHashSet();

        return new User(model.Id, model.FirstName, model.MiddleName, model.LastName, associations);
    }

    public static UserModel MapFrom(User entity)
    {
        var associations = entity.Associations
            .Select(UserAssociationsMapper.MapFrom)
            .ToHashSet();

        return new UserModel(entity.Id, entity.FirstName, entity.MiddleName, entity.LastName, associations);
    }
}