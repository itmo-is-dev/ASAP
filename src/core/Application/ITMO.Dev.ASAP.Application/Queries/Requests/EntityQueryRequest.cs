using ITMO.Dev.ASAP.Application.Dto.Querying;

namespace ITMO.Dev.ASAP.Application.Queries.Requests;

public record struct EntityQueryRequest<TBuilder, TParameter>(
    TBuilder QueryBuilder,
    QueryParameter<TParameter> Parameter);