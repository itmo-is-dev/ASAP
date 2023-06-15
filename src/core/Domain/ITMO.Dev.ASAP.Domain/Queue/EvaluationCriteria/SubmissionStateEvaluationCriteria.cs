using ITMO.Dev.ASAP.Domain.Models;
using ITMO.Dev.ASAP.Domain.Queue.Models;
using ITMO.Dev.ASAP.Domain.Submissions;

namespace ITMO.Dev.ASAP.Domain.Queue.EvaluationCriteria;

public class SubmissionStateEvaluationCriteria : IEvaluationCriteria
{
    private const double ActiveSubmissionPriority = 2;
    private const double ReviewedSubmissionPriority = 1;

    public SubmissionStateEvaluationCriteria(SortingOrder order)
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

    public double RateValue(SubmissionStateKind state)
    {
        return state switch
        {
            SubmissionStateKind.Active => ActiveSubmissionPriority,
            SubmissionStateKind.Reviewed => ReviewedSubmissionPriority,
            SubmissionStateKind.Inactive => 0D,
            SubmissionStateKind.Deleted => 0D,
            SubmissionStateKind.Completed => 0D,
            SubmissionStateKind.Banned => 0D,
            _ => 0D,
        };
    }
}