using FluentChaining;
using ITMO.Dev.ASAP.Application.Dto.Querying;
using ITMO.Dev.ASAP.Application.Queries.Requests;

namespace ITMO.Dev.ASAP.Application.Queries.Adapters;

public class EntityQueryAdapter<TEntity, TParameter> : IEntityQuery<TEntity, TParameter>
{
    private readonly IChain<EntityQueryRequest<TEntity, TParameter>, IQueryable<TEntity>> _chain;

    public EntityQueryAdapter(IChain<EntityQueryRequest<TEntity, TParameter>, IQueryable<TEntity>> chain)
    {
        _chain = chain;
    }

    public IQueryable<TEntity> Apply(IQueryable<TEntity> query, QueryConfiguration<TParameter> configuration)
    {
        foreach (QueryParameter<TParameter> parameter in configuration.Parameters)
        {
            var request = new EntityQueryRequest<TEntity, TParameter>(query, parameter);
            query = _chain.Process(request);
        }

        return query;
    }
}