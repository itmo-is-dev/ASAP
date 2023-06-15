using ITMO.Dev.ASAP.DataAccess.Models.Users;
using RichEntity.Annotations;

namespace ITMO.Dev.ASAP.DataAccess.Models.UserAssociations;

public abstract partial class UserAssociationModel : IEntity<Guid>
{
    protected UserAssociationModel(Guid id, Guid userId) : this(id)
    {
        UserId = userId;
    }

    public Guid UserId { get; set; }

    public virtual UserModel User { get; set; }
}