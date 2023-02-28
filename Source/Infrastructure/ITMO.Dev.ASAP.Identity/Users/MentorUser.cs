using ITMO.Dev.ASAP.Application.Abstractions.Identity;

namespace ITMO.Dev.ASAP.Identity.Users;

public class MentorUser : ICurrentUser
{
    public MentorUser(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; }
}