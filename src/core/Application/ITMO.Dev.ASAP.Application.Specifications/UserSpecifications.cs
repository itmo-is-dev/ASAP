using ITMO.Dev.ASAP.Application.DataAccess.Queries;
using ITMO.Dev.ASAP.Application.DataAccess.Repositories;
using ITMO.Dev.ASAP.Common.Exceptions;
using ITMO.Dev.ASAP.Domain.Users;

namespace ITMO.Dev.ASAP.Application.Specifications;

public static class UserSpecifications
{
    public static async Task<User?> FindByIdAsync(
        this IUserRepository repository,
        Guid id,
        CancellationToken cancellationToken)
    {
        var query = UserQuery.Build(x => x.WithId(id));

        return await repository.QueryAsync(query, cancellationToken).SingleOrDefaultAsync(cancellationToken);
    }

    public static async Task<User> GetByIdAsync(
        this IUserRepository repository,
        Guid id,
        CancellationToken cancellationToken)
    {
        User? user = await FindByIdAsync(repository, id, cancellationToken);

        return user ?? throw EntityNotFoundException.For<User>(id);
    }
}