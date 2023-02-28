using ITMO.Dev.ASAP.Application.Abstractions.Identity;

namespace ITMO.Dev.ASAP.Identity.Users;

public class AnonymousUser : ICurrentUser
{
    public AnonymousUser(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; }
}