using ITMO.Dev.ASAP.Application.Dto.Querying;

namespace ITMO.Dev.ASAP.Application.Queries.Requests;

public record struct EntityFilterRequest<TEntity, TParameter>(
    IEnumerable<TEntity> Data,
    QueryParameter<TParameter> Parameter);