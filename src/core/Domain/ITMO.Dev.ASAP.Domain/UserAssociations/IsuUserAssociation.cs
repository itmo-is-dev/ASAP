using ITMO.Dev.ASAP.Domain.Users;

namespace ITMO.Dev.ASAP.Domain.UserAssociations;

public partial class IsuUserAssociation : UserAssociation
{
    private IsuUserAssociation(Guid id, User user, int universityId) : base(id, user)
    {
        UniversityId = universityId;
    }

    public int UniversityId { get; set; }

    public static IsuUserAssociation CreateAndAttach(Guid id, User user, int universityId)
    {
        IsuUserAssociation association = new(id, user, universityId);
        association.AttachAssociation();

        return association;
    }
}