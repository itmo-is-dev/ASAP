using ITMO.Dev.ASAP.Domain.Models;
using ITMO.Dev.ASAP.Domain.Submissions;

namespace ITMO.Dev.ASAP.Domain.Queue.Evaluators;

public class SubmissionDateTimeEvaluator : ISubmissionEvaluator
{
    public SubmissionDateTimeEvaluator(SortingOrder sortingOrder)
    {
        SortingOrder = sortingOrder;
    }

    public SortingOrder SortingOrder { get; }

    public double Evaluate(Submission submission)
    {
        return submission.SubmissionDate.Value.Ticks;
    }
}