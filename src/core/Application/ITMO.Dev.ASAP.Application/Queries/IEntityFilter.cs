using ITMO.Dev.ASAP.Application.Dto.Querying;

namespace ITMO.Dev.ASAP.Application.Queries;

public interface IEntityFilter<TEntity, TParameter>
{
    IEnumerable<TEntity> Apply(
        IEnumerable<TEntity> data,
        QueryConfiguration<TParameter> configuration);
}