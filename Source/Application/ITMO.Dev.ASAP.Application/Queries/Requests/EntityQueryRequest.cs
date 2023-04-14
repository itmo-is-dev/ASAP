using ITMO.Dev.ASAP.Application.Dto.Querying;

namespace ITMO.Dev.ASAP.Application.Queries.Requests;

public record struct EntityQueryRequest<TEntity, TParameter>(
    IQueryable<TEntity> Query,
    QueryParameter<TParameter> Parameter);