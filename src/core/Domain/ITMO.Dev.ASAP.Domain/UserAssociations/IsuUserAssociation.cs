using ITMO.Dev.ASAP.Domain.Users;

namespace ITMO.Dev.ASAP.Domain.UserAssociations;

public partial class IsuUserAssociation : UserAssociation
{
    public IsuUserAssociation(Guid id, Guid userId, int universityId) : base(id, userId)
    {
        UniversityId = universityId;
    }

    public int UniversityId { get; set; }

    public static IsuUserAssociation CreateAndAttach(Guid id, User user, int universityId)
    {
        var association = new IsuUserAssociation(id, user.Id, universityId);
        user.AddAssociation(association);

        return association;
    }
}