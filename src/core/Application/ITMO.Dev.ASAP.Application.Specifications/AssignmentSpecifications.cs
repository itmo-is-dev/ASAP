using ITMO.Dev.ASAP.Application.DataAccess.Queries;
using ITMO.Dev.ASAP.Application.DataAccess.Repositories;
using ITMO.Dev.ASAP.Common.Exceptions;
using ITMO.Dev.ASAP.Domain.Study.Assignments;

namespace ITMO.Dev.ASAP.Application.Specifications;

public static class AssignmentSpecifications
{
    public static async Task<Assignment> GetByIdAsync(
        this IAssignmentRepository repository,
        Guid id,
        CancellationToken cancellationToken)
    {
        var query = AssignmentQuery.Build(x => x.WithId(id));

        Assignment? assignment = await repository
            .QueryAsync(query, cancellationToken)
            .SingleOrDefaultAsync(cancellationToken);

        return assignment ?? throw EntityNotFoundException.For<Assignment>(id);
    }

    public static IAsyncEnumerable<Assignment> GetByIdsAsync(
        this IAssignmentRepository repository,
        IEnumerable<Guid> ids,
        CancellationToken cancellationToken)
    {
        var query = AssignmentQuery.Build(x => x.WithIds(ids));
        return repository.QueryAsync(query, cancellationToken);
    }
}