namespace ITMO.Dev.ASAP.Domain.Queue;

public interface IFilterCriteria
{
    void Accept(IFilterCriteriaVisitor visitor);
}