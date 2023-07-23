using ITMO.Dev.ASAP.Common.Exceptions;
using ITMO.Dev.ASAP.Domain.Models;
using ITMO.Dev.ASAP.Domain.Queue.Models;
using ITMO.Dev.ASAP.Domain.Submissions;

namespace ITMO.Dev.ASAP.Domain.Queue;

public class SubmissionQueue
{
    private readonly IReadOnlyCollection<IFilterCriteria> _filters;
    private readonly IReadOnlyList<IEvaluationCriteria> _evaluators;

    public SubmissionQueue(
        IReadOnlyCollection<IFilterCriteria> filters,
        IReadOnlyList<IEvaluationCriteria> evaluators)
    {
        _filters = filters;
        _evaluators = evaluators;
    }

    public void AcceptFilterCriteriaVisitor(IFilterCriteriaVisitor visitor)
    {
        foreach (IFilterCriteria criteria in _filters)
        {
            criteria.Accept(visitor);
        }
    }

    public IAsyncEnumerable<Submission> OrderSubmissionsAsync(
        IAsyncEnumerable<Submission> submissions,
        IEvaluationCriteriaVisitor visitor,
        CancellationToken cancellationToken)
    {
        var iterator = new ForwardIterator<IEvaluationCriteria>(_evaluators, 0);

        return OrderSubmissionsAsync(submissions, visitor, iterator, cancellationToken);
    }

    private IAsyncEnumerable<Submission> OrderSubmissionsAsync(
        IAsyncEnumerable<Submission> submissions,
        IEvaluationCriteriaVisitor visitor,
        ForwardIterator<IEvaluationCriteria> iterator,
        CancellationToken cancellationToken)
    {
        IEvaluationCriteria evaluator = iterator.Current;

        IAsyncEnumerable<IAsyncGrouping<double, EvaluatedSubmission>> grouped = evaluator
            .AcceptAsync(submissions, visitor, cancellationToken)
            .GroupBy(s => s.Value);

        IOrderedAsyncEnumerable<IAsyncGrouping<double, EvaluatedSubmission>> ordered = evaluator.Order switch
        {
            SortingOrder.Ascending => grouped.OrderBy(x => x.Key),
            SortingOrder.Descending => grouped.OrderByDescending(x => x.Key),
            _ => throw new UnsupportedOperationException(nameof(evaluator.Order)),
        };

        if (iterator.IsAtEnd)
        {
            return ordered
                .SelectMany(x => x)
                .Select(x => x.Submission);
        }

        ForwardIterator<IEvaluationCriteria> next = iterator.Next();

        return ordered.SelectMany(g =>
        {
            IAsyncEnumerable<Submission> s = g.Select(x => x.Submission);
            return OrderSubmissionsAsync(s, visitor, next, cancellationToken);
        });
    }
}