using ITMO.Dev.ASAP.Domain.Deadlines.DeadlinePenalties;
using ITMO.Dev.ASAP.Domain.Groups;
using ITMO.Dev.ASAP.Domain.Study.Assignments;
using ITMO.Dev.ASAP.Domain.SubmissionStateWorkflows;
using ITMO.Dev.ASAP.Domain.Users;

namespace ITMO.Dev.ASAP.Domain.Study.SubjectCourses;

public class SubjectCourseBuilder
{
    private readonly Guid _id;
    private readonly string _title;
    private readonly SubmissionStateWorkflowType? _workflowType;

    public SubjectCourseBuilder(Guid id, string title, SubmissionStateWorkflowType? workflowType)
    {
        _id = id;
        _title = title;
        _workflowType = workflowType;
    }

    public SubjectCourse Build(Subject subject)
    {
        // TODO: change when deadline penalty customization is implemented
        var penalties = Enumerable
            .Range(0, 5)
            .Select<int, DeadlinePenalty>(
                i => new FractionDeadlinePenalty(TimeSpan.FromDays(7) * i, 1 - (0.2 * (i + 1))))
            .ToHashSet();

        return new SubjectCourse(
            _id,
            subject.Id,
            _title,
            _workflowType,
            new HashSet<StudentGroupInfo>(),
            penalties,
            new HashSet<SubjectCourseAssignment>(),
            new HashSet<Mentor>());
    }
}