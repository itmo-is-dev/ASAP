using ITMO.Dev.ASAP.Common.Exceptions;
using ITMO.Dev.ASAP.Core.Deadlines.DeadlinePenalties;
using ITMO.Dev.ASAP.Core.Submissions;
using ITMO.Dev.ASAP.Core.ValueObject;

namespace ITMO.Dev.ASAP.Domain.Deadlines.DeadlinePolicies;

public class DeadlinePolicy
{
    private readonly HashSet<DeadlinePenalty> _deadlinePenalties;

    public DeadlinePolicy()
    {
        _deadlinePenalties = new HashSet<DeadlinePenalty>();
    }

    public void AddDeadlinePolicy(DeadlinePenalty penalty)
    {
        ArgumentNullException.ThrowIfNull(penalty);

        if (!_deadlinePenalties.Add(penalty))
            throw new DomainInvalidOperationException($"Deadline span {penalty} already exists");
    }

    public void RemoveDeadlinePolicy(DeadlinePenalty penalty)
    {
        ArgumentNullException.ThrowIfNull(penalty);

        if (!_deadlinePenalties.Remove(penalty))
            throw new DomainInvalidOperationException($"Deadline span {penalty} cannot be removed");
    }

    public Points? GetPointPenalty(Submission submission)
    {
        ArgumentNullException.ThrowIfNull(submission);

        if (submission.Points is null)
            return null;

        Points points = submission.Points.Value;
        DeadlinePenalty? deadlinePolicy = GetEffectiveDeadlinePolicy(submission);

        return deadlinePolicy?.Apply(points);
    }

    private DeadlinePenalty? GetEffectiveDeadlinePolicy(Submission submission)
    {
        DateOnly deadline = submission.GroupAssignment.Deadline;

        if (submission.SubmissionDateOnly <= deadline)
            return null;

        var submissionDeadlineOffset = TimeSpan.FromDays(
            submission.SubmissionDateOnly.DayNumber - deadline.DayNumber);

        DeadlinePenalty? activeDeadlinePolicy = _deadlinePenalties
            .Where(dp => dp.SpanBeforeActivation < submissionDeadlineOffset)
            .MaxBy(dp => dp.SpanBeforeActivation);

        return activeDeadlinePolicy;
    }
}