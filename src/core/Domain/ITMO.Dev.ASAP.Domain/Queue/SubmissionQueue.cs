using ITMO.Dev.ASAP.Common.Exceptions;
using ITMO.Dev.ASAP.Domain.Models;
using ITMO.Dev.ASAP.Domain.Queue.Evaluators;
using ITMO.Dev.ASAP.Domain.Queue.Filters;
using ITMO.Dev.ASAP.Domain.Submissions;
using RichEntity.Annotations;

namespace ITMO.Dev.ASAP.Domain.Queue;

public partial class SubmissionQueue : IEntity<Guid>
{
    private readonly IReadOnlyList<ISubmissionEvaluator> _evaluators;
    private readonly IReadOnlyCollection<IQueueFilter> _filters;

    public SubmissionQueue(
        IReadOnlyCollection<IQueueFilter> filters,
        IReadOnlyList<ISubmissionEvaluator> evaluators)
        : this(Guid.NewGuid())
    {
        ArgumentNullException.ThrowIfNull(filters);
        ArgumentNullException.ThrowIfNull(evaluators);

        _filters = filters;
        _evaluators = evaluators;
    }

    public async Task<IEnumerable<Submission>> UpdateSubmissions(
        IQueryable<Submission> query,
        IQueryExecutor queryExecutor,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);
        ArgumentNullException.ThrowIfNull(queryExecutor);

        query = _filters.Aggregate(query, (current, filter) => filter.Filter(current));
        IReadOnlyCollection<Submission> submissions = await queryExecutor.ExecuteAsync(query, cancellationToken);

        return SortedBy(submissions, _evaluators);
    }

    private static IEnumerable<Submission> SortedBy(
        IEnumerable<Submission> submissions,
        IReadOnlyList<ISubmissionEvaluator> evaluators)
    {
        var stepperEvaluators = new ForwardIterator<ISubmissionEvaluator>(evaluators, 0);
        return SortedBy(submissions, stepperEvaluators);
    }

    private static IEnumerable<Submission> SortedBy(
        IEnumerable<Submission> submissions,
        ForwardIterator<ISubmissionEvaluator> evaluators)
    {
        ISubmissionEvaluator evaluator = evaluators.Current;

        IEnumerable<IGrouping<double, Submission>> groupings = submissions
            .GroupBy(x => evaluator.Evaluate(x));

        IOrderedEnumerable<IGrouping<double, Submission>> orderedGroupings = evaluator.SortingOrder switch
        {
            SortingOrder.Ascending => groupings.OrderBy(x => x.Key),
            SortingOrder.Descending => groupings.OrderByDescending(x => x.Key),
            _ => throw new UnsupportedOperationException(nameof(evaluator.SortingOrder)),
        };

        if (evaluators.IsAtEnd)
            return orderedGroupings.SelectMany(x => x);

        ForwardIterator<ISubmissionEvaluator> next = evaluators.Next();
        return orderedGroupings.SelectMany(x => SortedBy(x, next));
    }
}