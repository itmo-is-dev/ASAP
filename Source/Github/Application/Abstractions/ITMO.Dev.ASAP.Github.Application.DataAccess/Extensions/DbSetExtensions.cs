using ITMO.Dev.ASAP.Github.Common.Exceptions.Entities;
using ITMO.Dev.ASAP.Github.Common.Extensions;
using Microsoft.EntityFrameworkCore;
using RichEntity.Annotations;

namespace ITMO.Dev.ASAP.Github.Application.DataAccess.Extensions;

public static class DbSetExtensions
{
    public static async Task<T> GetByIdAsync<T, TKey>(
        this DbSet<T> dbSet,
        TKey id,
        CancellationToken cancellationToken = default)
        where T : class
        where TKey : IEquatable<TKey>
    {
        T? entity = await dbSet.FindAsync(new object[] { id }, cancellationToken);
        return entity ?? throw EntityNotFoundException.Create<TKey, T>(id).TaggedWithNotFound();
    }

    public static async Task<T> GetByIdAsync<T, TKey>(
        this IQueryable<T> dbSet,
        TKey id,
        CancellationToken cancellationToken = default)
        where T : class, IEntity<TKey>
        where TKey : IEquatable<TKey>
    {
        T? entity = await dbSet.SingleOrDefaultAsync(x => x.Id.Equals(id), cancellationToken);
        return entity ?? throw EntityNotFoundException.Create<TKey, T>(id).TaggedWithNotFound();
    }
}