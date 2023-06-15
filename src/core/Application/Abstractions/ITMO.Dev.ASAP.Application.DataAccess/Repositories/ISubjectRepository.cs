using ITMO.Dev.ASAP.Application.DataAccess.Queries;
using ITMO.Dev.ASAP.Domain.Study;

namespace ITMO.Dev.ASAP.Application.DataAccess.Repositories;

public interface ISubjectRepository
{
    IAsyncEnumerable<Subject> QueryAsync(SubjectQuery query, CancellationToken cancellationToken);

    void Add(Subject subject);

    void Update(Subject subject);
}