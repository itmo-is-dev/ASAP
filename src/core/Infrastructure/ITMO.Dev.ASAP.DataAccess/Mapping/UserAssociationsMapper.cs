using ITMO.Dev.ASAP.DataAccess.Models.UserAssociations;
using ITMO.Dev.ASAP.Domain.UserAssociations;

namespace ITMO.Dev.ASAP.DataAccess.Mapping;

public static class UserAssociationsMapper
{
    public static UserAssociation MapTo(UserAssociationModel model)
    {
        return model switch
        {
            IsuUserAssociationModel association
                => new IsuUserAssociation(association.Id, association.UserId, association.UniversityId),

            _ => throw new ArgumentOutOfRangeException(nameof(model)),
        };
    }

    public static UserAssociationModel MapFrom(UserAssociation entity)
    {
        return entity switch
        {
            IsuUserAssociation association
                => new IsuUserAssociationModel(association.Id, association.UserId, association.UniversityId),

            _ => throw new ArgumentOutOfRangeException(nameof(entity)),
        };
    }
}