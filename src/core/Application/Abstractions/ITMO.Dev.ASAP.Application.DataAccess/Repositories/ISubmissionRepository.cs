using ITMO.Dev.ASAP.Application.DataAccess.Queries;
using ITMO.Dev.ASAP.Domain.Submissions;

namespace ITMO.Dev.ASAP.Application.DataAccess.Repositories;

public interface ISubmissionRepository
{
    IAsyncEnumerable<Submission> QueryAsync(SubmissionQuery query, CancellationToken cancellationToken);

    Task<int> CountAsync(SubmissionQuery query, CancellationToken cancellationToken);

    void Add(Submission submission);

    void Update(Submission submission);
}