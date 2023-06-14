using ITMO.Dev.ASAP.Application.DataAccess.Queries;
using ITMO.Dev.ASAP.Domain.Study.GroupAssignments;

namespace ITMO.Dev.ASAP.Application.DataAccess.Repositories;

public interface IGroupAssignmentRepository
{
    IAsyncEnumerable<GroupAssignment> QueryAsync(GroupAssignmentQuery query, CancellationToken cancellationToken);
}