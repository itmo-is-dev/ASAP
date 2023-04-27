using ITMO.Dev.ASAP.Domain.Models;
using ITMO.Dev.ASAP.Domain.Queue.Evaluators;
using ITMO.Dev.ASAP.Domain.Queue.Filters;
using ITMO.Dev.ASAP.Domain.Study;
using ITMO.Dev.ASAP.Domain.Submissions.States;

namespace ITMO.Dev.ASAP.Domain.Queue.Building;

public class DefaultQueueBuilder : QueueBuilder
{
    public DefaultQueueBuilder(StudentGroup group, Guid subjectCourseId)
    {
        AddFilter(new GroupQueueFilter(new[] { group }));
        AddFilter(new SubmissionStateFilter(new ActiveSubmissionState(), new ReviewedSubmissionState()));
        AddFilter(new SubjectCoursesFilter(subjectCourseId));

        AddEvaluator(new SubmissionStateEvaluator(SortingOrder.Descending));
        AddEvaluator(new AssignmentDeadlineStateEvaluator(SortingOrder.Descending));
        AddEvaluator(new SubmissionDateTimeEvaluator(SortingOrder.Ascending));
    }
}