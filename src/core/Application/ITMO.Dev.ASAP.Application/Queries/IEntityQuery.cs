using ITMO.Dev.ASAP.Application.Dto.Querying;

namespace ITMO.Dev.ASAP.Application.Queries;

public interface IEntityQuery<TBuilder, TParameter>
{
    TBuilder Apply(TBuilder queryBuilder, QueryConfiguration<TParameter> configuration);
}