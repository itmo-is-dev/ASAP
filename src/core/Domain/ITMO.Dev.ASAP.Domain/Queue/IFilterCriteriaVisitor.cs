using ITMO.Dev.ASAP.Domain.Queue.FilterCriteria;

namespace ITMO.Dev.ASAP.Domain.Queue;

public interface IFilterCriteriaVisitor
{
    void Visit(AssignmentFilterCriteria criteria);

    void Visit(StudentGroupFilterCriteria criteria);

    void Visit(SubjectCourseFilterCriteria criteria);

    void Visit(SubmissionStateFilterCriteria criteria);
}