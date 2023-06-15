using ITMO.Dev.ASAP.Domain.Deadlines.DeadlinePenalties;
using ITMO.Dev.ASAP.Domain.Models;
using ITMO.Dev.ASAP.Domain.Students;
using ITMO.Dev.ASAP.Domain.Study.Assignments;
using ITMO.Dev.ASAP.Domain.Study.SubjectCourses;
using ITMO.Dev.ASAP.Domain.Submissions;
using ITMO.Dev.ASAP.Domain.ValueObject;
using RichEntity.Annotations;

namespace ITMO.Dev.ASAP.Domain.Study;

public partial class StudentAssignment : IEntity
{
    public StudentAssignment(
        Student student,
        Assignment assignment,
        IReadOnlyCollection<GroupAssignments.GroupAssignment> groupAssignments,
        IReadOnlyCollection<Submission> submissions,
        SubjectCourse subjectCourse)
        : this(assignment.Id, student.UserId)
    {
        Student = student;
        Assignment = assignment;
        GroupAssignments = groupAssignments;
        Submissions = submissions;
        SubjectCourse = subjectCourse;
    }

    [KeyProperty]
    public Student Student { get; }

    [KeyProperty]
    public Assignment Assignment { get; }

    public IReadOnlyCollection<GroupAssignments.GroupAssignment> GroupAssignments { get; }

    public IReadOnlyCollection<Submission> Submissions { get; }

    public SubjectCourse SubjectCourse { get; }

    public StudentAssignmentPoints? CalculatePoints()
    {
        IEnumerable<Submission> submissions = Submissions
            .Where(x => x.State.IsTerminalEffectiveState);

        (Submission submission, Points? points, bool isBanned) = submissions
            .Select(s => (
                submission: s,
                points: GetEffectivePoints(s),
                isBanned: s.State.Kind is SubmissionStateKind.Banned))
            .OrderByDescending(x => x.isBanned)
            .ThenByDescending(x => x.points)
            .FirstOrDefault();

        if (points is null && isBanned is false)
            return null;

        if (isBanned)
            points = null;

        return new StudentAssignmentPoints(
            Student,
            Assignment,
            isBanned,
            points ?? Points.None,
            GetPointPenalty(submission) ?? Points.None,
            submission.SubmissionDateOnly);
    }

    private Points? GetEffectivePoints(Submission submission)
    {
        if (submission.Rating is null)
            return null;

        Points points = Assignment.MaxPoints * submission.Rating.Value;
        DeadlinePenalty? deadlinePenalty = GetEffectiveDeadlinePenalty(submission);

        if (deadlinePenalty is not null)
            points = deadlinePenalty.Apply(points);

        if (submission.ExtraPoints is not null)
            points += submission.ExtraPoints.Value;

        return points;
    }

    private Points? GetPointPenalty(Submission submission)
    {
        if (submission.Rating is null)
            return null;

        Points? deadlineAppliedPoints = GetEffectivePoints(submission);

        if (deadlineAppliedPoints is null)
            return null;

        Points? points = Assignment.MaxPoints * submission.Rating;
        Points? penaltyPoints = points - deadlineAppliedPoints;

        return penaltyPoints;
    }

    private DeadlinePenalty? GetEffectiveDeadlinePenalty(Submission submission)
    {
        GroupAssignments.GroupAssignment groupAssignment = GroupAssignments
            .Single(ga => ga.Id.StudentGroupId.Equals(submission.GroupAssignment.Id.StudentGroupId));

        DateOnly deadline = groupAssignment.Deadline;

        return SubjectCourse.DeadlinePolicy.FindEffectiveDeadlinePenalty(deadline, submission.SubmissionDateOnly);
    }
}