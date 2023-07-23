using ITMO.Dev.ASAP.Domain.Models;

namespace ITMO.Dev.ASAP.Domain.Queue.FilterCriteria;

public class SubmissionStateFilterCriteria : IFilterCriteria
{
    public SubmissionStateFilterCriteria(params SubmissionStateKind[] states)
    {
        States = states;
    }

    public IReadOnlyCollection<SubmissionStateKind> States { get; }

    public void Accept(IFilterCriteriaVisitor visitor)
    {
        visitor.Visit(this);
    }
}