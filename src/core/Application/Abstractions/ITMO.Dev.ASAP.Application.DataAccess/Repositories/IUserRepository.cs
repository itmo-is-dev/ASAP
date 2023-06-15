using ITMO.Dev.ASAP.Application.DataAccess.Queries;
using ITMO.Dev.ASAP.Domain.Users;

namespace ITMO.Dev.ASAP.Application.DataAccess.Repositories;

public interface IUserRepository
{
    IAsyncEnumerable<User> QueryAsync(UserQuery query, CancellationToken cancellationToken);

    Task<long> CountAsync(UserQuery query, CancellationToken cancellationToken);

    void Add(User user);

    void Update(User user);

    void AddRange(IEnumerable<User> users);
}