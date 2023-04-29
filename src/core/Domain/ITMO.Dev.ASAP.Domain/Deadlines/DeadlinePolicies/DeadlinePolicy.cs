using ITMO.Dev.ASAP.Common.Exceptions;
using ITMO.Dev.ASAP.Core.Deadlines.DeadlinePenalties;
using ITMO.Dev.ASAP.Core.ValueObject;
using RichEntity.Annotations;

namespace ITMO.Dev.ASAP.Core.Deadlines.DeadlinePolicies;

public partial class DeadlinePolicy : IEntity<Guid>
{
    private readonly HashSet<DeadlinePenalty> _deadlinePenalties;

    public DeadlinePolicy(Guid id)
    {
        Id = id;
        _deadlinePenalties = new HashSet<DeadlinePenalty>();
    }

    public virtual IReadOnlyCollection<DeadlinePenalty> DeadlinePenalties => _deadlinePenalties;

    public void AddDeadlinePenalty(DeadlinePenalty penalty)
    {
        ArgumentNullException.ThrowIfNull(penalty);

        if (_deadlinePenalties.Add(penalty) is false)
            throw new DomainInvalidOperationException($"Deadline span {penalty} already exists");
    }

    public void RemoveDeadlinePenalty(DeadlinePenalty penalty)
    {
        ArgumentNullException.ThrowIfNull(penalty);

        if (_deadlinePenalties.Remove(penalty) is false)
            throw new DomainInvalidOperationException($"Deadline span {penalty} cannot be removed");
    }

    public Points? GetPointPenalty(Points? points, DateOnly deadline, DateOnly submissionDate)
    {
        if (points is null)
            return null;

        DeadlinePenalty? deadlinePenalty = GetEffectiveDeadlinePenalty(deadline, submissionDate);

        return deadlinePenalty?.Apply(points.Value);
    }

    private DeadlinePenalty? GetEffectiveDeadlinePenalty(DateOnly deadline, DateOnly submissionDate)
    {
        if (submissionDate <= deadline)
            return null;

        var submissionDeadlineOffset = TimeSpan.FromDays(
            submissionDate.DayNumber - deadline.DayNumber);

        DeadlinePenalty? activeDeadlinePenalty = _deadlinePenalties
            .Where(dp => dp.SpanBeforeActivation < submissionDeadlineOffset)
            .MaxBy(dp => dp.SpanBeforeActivation);

        return activeDeadlinePenalty;
    }
}