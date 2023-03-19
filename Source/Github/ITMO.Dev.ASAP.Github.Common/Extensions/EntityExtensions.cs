using ITMO.Dev.ASAP.Github.Common.Exceptions.Entities;

namespace ITMO.Dev.ASAP.Github.Common.Extensions;

public static class EntityExtensions
{
    public static async Task<TEntity> ThrowTaggedEntityNotFoundIfNull<TKey, TEntity>(
        this Task<TEntity?> task,
        TKey key)
    {
        TEntity? entity = await task;

        if (entity is not null)
            return entity;

        throw EntityNotFoundException.Create<TKey, TEntity>(key).TaggedWithNotFound();
    }
}