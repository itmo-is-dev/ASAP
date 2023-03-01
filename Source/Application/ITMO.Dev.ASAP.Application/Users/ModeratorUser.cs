using ITMO.Dev.ASAP.Application.Abstractions.Identity;

namespace ITMO.Dev.ASAP.Application.Users;

public class ModeratorUser : ICurrentUser
{
    public ModeratorUser(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; }
}