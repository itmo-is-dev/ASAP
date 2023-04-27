using ITMO.Dev.ASAP.Domain.Models;
using ITMO.Dev.ASAP.Domain.Submissions;

namespace ITMO.Dev.ASAP.Domain.Queue.Evaluators;

public interface ISubmissionEvaluator
{
    SortingOrder SortingOrder { get; }

    double Evaluate(Submission submission);
}