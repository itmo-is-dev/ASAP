using ITMO.Dev.ASAP.Application.DataAccess.Queries;
using ITMO.Dev.ASAP.Domain.Users;

namespace ITMO.Dev.ASAP.Application.DataAccess.Repositories;

public interface IMentorRepository
{
    IAsyncEnumerable<Mentor> QueryAsync(MentorQuery query, CancellationToken cancellationToken);
}