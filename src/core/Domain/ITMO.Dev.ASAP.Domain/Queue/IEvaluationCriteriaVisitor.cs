using ITMO.Dev.ASAP.Domain.Queue.EvaluationCriteria;
using ITMO.Dev.ASAP.Domain.Queue.Models;
using ITMO.Dev.ASAP.Domain.Submissions;

namespace ITMO.Dev.ASAP.Domain.Queue;

public interface IEvaluationCriteriaVisitor
{
    IAsyncEnumerable<EvaluatedSubmission> VisitAsync(
        IAsyncEnumerable<Submission> submissions,
        AssignmentDeadlineEvaluationCriteria criteria,
        CancellationToken cancellationToken);

    IAsyncEnumerable<EvaluatedSubmission> VisitAsync(
        IAsyncEnumerable<Submission> submissions,
        SubmissionDateTimeEvaluationCriteria criteria,
        CancellationToken cancellationToken);

    IAsyncEnumerable<EvaluatedSubmission> VisitAsync(
        IAsyncEnumerable<Submission> submissions,
        SubmissionStateEvaluationCriteria criteria,
        CancellationToken cancellationToken);
}