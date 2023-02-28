using ITMO.Dev.ASAP.Application.Abstractions.Identity;

namespace ITMO.Dev.ASAP.Identity.Users;

public class AdminUser : ICurrentUser
{
    public AdminUser(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; }
}