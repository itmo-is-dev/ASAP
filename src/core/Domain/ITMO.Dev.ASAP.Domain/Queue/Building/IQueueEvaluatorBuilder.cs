using ITMO.Dev.ASAP.Domain.Queue.Evaluators;
using ITMO.Dev.ASAP.Domain.Queue.Filters;

namespace ITMO.Dev.ASAP.Domain.Queue.Building;

public interface IQueueEvaluatorBuilder
{
    IQueueEvaluatorBuilder AddEvaluator(ISubmissionEvaluator evaluator);

    SubmissionQueue Build();
}

public interface IQueueFilterBuilder : IQueueEvaluatorBuilder
{
    IQueueFilterBuilder AddFilter(IQueueFilter filter);
}