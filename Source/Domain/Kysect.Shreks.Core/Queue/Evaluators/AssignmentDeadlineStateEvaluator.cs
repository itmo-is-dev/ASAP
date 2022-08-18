using Kysect.Shreks.Core.Models;
using Kysect.Shreks.Core.Study;

namespace Kysect.Shreks.Core.Queue.Evaluators;

public partial class AssignmentDeadlineStateEvaluator : SubmissionEvaluator
{
    private const double CurrentAssignmentPriority = 2;
    private const double ExpiredAssignmentPriority = 1;
    private const double OtherAssignmentPriority = 0;
    
    public AssignmentDeadlineStateEvaluator(int position, SortingOrder sortingOrder) : base(position, sortingOrder) { }

    public override double Evaluate(Submission submission)
    {
        ArgumentNullException.ThrowIfNull(submission);

        var groupAssignment = submission.Assignment
            .GroupAssignments
            .Single(x => x.Group.Equals(submission.Student.Group));

        if (groupAssignment.Deadline < submission.SubmissionDateTime)
            return ExpiredAssignmentPriority;

        var now = DateOnly.FromDateTime(DateTime.Now);

        var closestDeadline = submission
            .Assignment
            .SubjectCourse
            .Assignments
            .SelectMany(x => x.GroupAssignments)
            .Where(x => x.Group.Equals(submission.Student.Group))
            .Select(x => x.Deadline)
            .Where(x => x > now)
            .Min();

        return groupAssignment.Deadline.Equals(closestDeadline) ? CurrentAssignmentPriority : OtherAssignmentPriority;
    }
}