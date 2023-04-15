using RichEntity.Annotations;

namespace ITMO.Dev.ASAP.Github.Application.Specifications;

public static class EntitySpecifications
{
    public static IQueryable<TEntity> WithId<TEntity, TKey>(this IQueryable<TEntity> queryable, TKey key)
        where TEntity : IEntity<TKey>
        where TKey : IComparable<TKey>
    {
        return queryable.Where(x => x.Id.Equals(key));
    }

    public static IQueryable<TEntity> WithIds<TEntity, TKey>(this IQueryable<TEntity> queryable, IEnumerable<TKey> keys)
        where TEntity : IEntity<TKey>
        where TKey : IComparable<TKey>
    {
        return queryable.Where(x => keys.Contains(x.Id));
    }
}