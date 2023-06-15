namespace ITMO.Dev.ASAP.Domain.Queue.FilterCriteria;

public class AssignmentFilterCriteria : IFilterCriteria
{
    public AssignmentFilterCriteria(IReadOnlyCollection<Guid> assignmentIds)
    {
        AssignmentIds = assignmentIds;
    }

    public IReadOnlyCollection<Guid> AssignmentIds { get; }

    public void Accept(IFilterCriteriaVisitor visitor)
    {
        visitor.Visit(this);
    }
}