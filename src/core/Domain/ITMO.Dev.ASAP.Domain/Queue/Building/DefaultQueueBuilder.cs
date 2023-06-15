using ITMO.Dev.ASAP.Domain.Models;
using ITMO.Dev.ASAP.Domain.Queue.EvaluationCriteria;
using ITMO.Dev.ASAP.Domain.Queue.FilterCriteria;
using ITMO.Dev.ASAP.Domain.Queue.Models;

namespace ITMO.Dev.ASAP.Domain.Queue.Building;

public class DefaultQueueBuilder : QueueBuilder
{
    public DefaultQueueBuilder(
        Guid studentGroupId,
        Guid subjectCourseId)
    {
        AddFilter(new StudentGroupFilterCriteria(new[] { studentGroupId }));
        AddFilter(new SubmissionStateFilterCriteria(SubmissionStateKind.Active, SubmissionStateKind.Reviewed));
        AddFilter(new SubjectCourseFilterCriteria(subjectCourseId));

        AddEvaluator(new SubmissionStateEvaluationCriteria(SortingOrder.Descending));
        AddEvaluator(new AssignmentDeadlineEvaluationCriteria(SortingOrder.Descending));
        AddEvaluator(new SubmissionDateTimeEvaluationCriteria(SortingOrder.Ascending));
    }
}