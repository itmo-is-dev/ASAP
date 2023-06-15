namespace ITMO.Dev.ASAP.Domain.Queue.FilterCriteria;

public class SubjectCourseFilterCriteria : IFilterCriteria
{
    public SubjectCourseFilterCriteria(Guid subjectCourseId)
    {
        SubjectCourseId = subjectCourseId;
    }

    public Guid SubjectCourseId { get; }

    public void Accept(IFilterCriteriaVisitor visitor)
    {
        visitor.Visit(this);
    }
}