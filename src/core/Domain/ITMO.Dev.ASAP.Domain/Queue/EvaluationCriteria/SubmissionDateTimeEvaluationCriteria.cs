using ITMO.Dev.ASAP.Domain.Queue.Models;
using ITMO.Dev.ASAP.Domain.Submissions;

namespace ITMO.Dev.ASAP.Domain.Queue.EvaluationCriteria;

public class SubmissionDateTimeEvaluationCriteria : IEvaluationCriteria
{
    public SubmissionDateTimeEvaluationCriteria(SortingOrder order)
    {
        Order = order;
    }

    public SortingOrder Order { get; }

    public IAsyncEnumerable<EvaluatedSubmission> AcceptAsync(
        IAsyncEnumerable<Submission> submissions,
        IEvaluationCriteriaVisitor visitor,
        CancellationToken cancellationToken)
    {
        return visitor.VisitAsync(submissions, this, cancellationToken);
    }
}