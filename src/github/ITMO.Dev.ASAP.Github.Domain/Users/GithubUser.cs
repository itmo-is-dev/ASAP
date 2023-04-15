using RichEntity.Annotations;

namespace ITMO.Dev.ASAP.Github.Domain.Users;

public partial class GithubUser : IEntity<Guid>
{
    public GithubUser(Guid id, string username) : this(id)
    {
        Username = username;
    }

    public string Username { get; set; }
}