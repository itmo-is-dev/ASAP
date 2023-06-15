using ITMO.Dev.ASAP.Domain.Queue.Models;
using ITMO.Dev.ASAP.Domain.Submissions;

namespace ITMO.Dev.ASAP.Domain.Queue;

public interface IEvaluationCriteria
{
    SortingOrder Order { get; }

    IAsyncEnumerable<EvaluatedSubmission> AcceptAsync(
        IAsyncEnumerable<Submission> submissions,
        IEvaluationCriteriaVisitor visitor,
        CancellationToken cancellationToken);
}