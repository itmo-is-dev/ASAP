namespace ITMO.Dev.ASAP.Domain.Queue.Building;

public class QueueBuilder : IQueueFilterBuilder
{
    private readonly List<IEvaluationCriteria> _evaluators;
    private readonly List<IFilterCriteria> _filters;

    public QueueBuilder()
    {
        _filters = new List<IFilterCriteria>();
        _evaluators = new List<IEvaluationCriteria>();
    }

    public IQueueFilterBuilder AddFilter(IFilterCriteria filter)
    {
        _filters.Add(filter);
        return this;
    }

    public IQueueEvaluatorBuilder AddEvaluator(IEvaluationCriteria evaluator)
    {
        _evaluators.Add(evaluator);
        return this;
    }

    public SubmissionQueue Build()
    {
        IFilterCriteria[] filters = _filters.ToArray();
        IEvaluationCriteria[] evaluators = _evaluators.ToArray();

        return new SubmissionQueue(filters, evaluators);
    }
}