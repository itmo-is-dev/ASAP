using FluentChaining;
using ITMO.Dev.ASAP.Application.Dto.Querying;
using ITMO.Dev.ASAP.Application.Queries.Requests;

namespace ITMO.Dev.ASAP.Application.Queries.Adapters;

public class EntityFilterAdapter<TEntity, TParameter> : IEntityFilter<TEntity, TParameter>
{
    private readonly IChain<EntityFilterRequest<TEntity, TParameter>, IEnumerable<TEntity>> _chain;

    public EntityFilterAdapter(IChain<EntityFilterRequest<TEntity, TParameter>, IEnumerable<TEntity>> chain)
    {
        _chain = chain;
    }

    public IEnumerable<TEntity> Apply(IEnumerable<TEntity> data, QueryConfiguration<TParameter> configuration)
    {
        foreach (QueryParameter<TParameter> parameter in configuration.Parameters)
        {
            var request = new EntityFilterRequest<TEntity, TParameter>(data, parameter);
            data = _chain.Process(request);
        }

        return data;
    }
}