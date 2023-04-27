using ITMO.Dev.ASAP.Domain.Users;
using RichEntity.Annotations;

namespace ITMO.Dev.ASAP.Domain.UserAssociations;

public abstract partial class UserAssociation : IEntity<Guid>
{
    protected UserAssociation(Guid id, User user) : this(id)
    {
        User = user;
        user.AddAssociation(this);
    }

    public virtual User User { get; protected init; }
}