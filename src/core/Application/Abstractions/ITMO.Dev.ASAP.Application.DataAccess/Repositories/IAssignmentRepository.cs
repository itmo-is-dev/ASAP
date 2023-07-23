using ITMO.Dev.ASAP.Application.DataAccess.Queries;
using ITMO.Dev.ASAP.Domain.Study.Assignments;

namespace ITMO.Dev.ASAP.Application.DataAccess.Repositories;

public interface IAssignmentRepository
{
    IAsyncEnumerable<Assignment> QueryAsync(AssignmentQuery query, CancellationToken cancellationToken);

    void Update(Assignment assignment);
}