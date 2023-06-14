using ITMO.Dev.ASAP.Application.DataAccess.Queries;
using ITMO.Dev.ASAP.Domain.Groups;

namespace ITMO.Dev.ASAP.Application.DataAccess.Repositories;

public interface IStudentGroupRepository
{
    IAsyncEnumerable<StudentGroup> QueryAsync(StudentGroupQuery query, CancellationToken cancellationToken);

    void Update(StudentGroup studentGroup);

    void Add(StudentGroup studentGroup);
}