using ITMO.Dev.ASAP.Application.DataAccess.Queries;
using ITMO.Dev.ASAP.Application.DataAccess.Repositories;
using ITMO.Dev.ASAP.Common.Exceptions;
using ITMO.Dev.ASAP.Domain.Study;

namespace ITMO.Dev.ASAP.Application.Specifications;

public static class SubjectSpecifications
{
    public static async Task<Subject> GetByIdAsync(
        this ISubjectRepository repository,
        Guid id,
        CancellationToken cancellationToken)
    {
        var query = SubjectQuery.Build(x => x.WithId(id));

        Subject? subject = await repository
            .QueryAsync(query, cancellationToken)
            .SingleOrDefaultAsync(cancellationToken);

        return subject ?? throw EntityNotFoundException.For<Subject>(id);
    }
}