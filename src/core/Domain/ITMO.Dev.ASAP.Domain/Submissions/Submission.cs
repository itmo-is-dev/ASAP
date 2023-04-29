using ITMO.Dev.ASAP.Common.Exceptions;
using ITMO.Dev.ASAP.Domain.Deadlines.DeadlinePenalties;
using ITMO.Dev.ASAP.Domain.Study;
using ITMO.Dev.ASAP.Domain.Submissions.States;
using ITMO.Dev.ASAP.Domain.Tools;
using ITMO.Dev.ASAP.Domain.Users;
using ITMO.Dev.ASAP.Domain.ValueObject;
using RichEntity.Annotations;

namespace ITMO.Dev.ASAP.Domain.Submissions;

public partial class Submission : IEntity<Guid>
{
    public Submission(
        Guid id,
        int code,
        Student student,
        GroupAssignment groupAssignment,
        SpbDateTime submissionDate,
        string payload)
        : this(id)
    {
        Code = code;
        SubmissionDate = submissionDate;
        Student = student;
        GroupAssignment = groupAssignment;
        Payload = payload;

        Rating = default;
        ExtraPoints = default;

        State = new ActiveSubmissionState();
    }

    public int Code { get; protected init; }

    public string Payload { get; set; }

    public Fraction? Rating { get; private set; }

    public Points? ExtraPoints { get; set; }

    public SpbDateTime SubmissionDate { get; private set; }

    public virtual Student Student { get; protected init; }

    public virtual GroupAssignment GroupAssignment { get; protected init; }

    public Points? Points => Rating is null ? default : GroupAssignment.Assignment.MaxPoints * Rating;

    /// <summary>
    ///     Gets points with deadline policy applied.
    /// </summary>
    public Points? EffectivePoints => GetEffectivePoints();

    /// <summary>
    ///     Gets points subtracted by deadline policy.
    /// </summary>
    public Points? PointPenalty => GetPointPenalty();

    public bool IsRated => Rating is not null;

    public DateOnly SubmissionDateOnly => SubmissionDate.AsDateOnly();

    public ISubmissionState State { get; private set; }

    public override string ToString()
    {
        return $"{Code} ({Id})";
    }

    public void Rate(Fraction? rating, Points? extraPoints)
    {
        if (rating is null && extraPoints is null)
        {
            const string ratingName = nameof(rating);
            const string extraPointsName = nameof(extraPoints);
            const string message =
                $"Cannot update submission points, both {ratingName} and {extraPointsName} are null.";
            throw new DomainInvalidOperationException(message);
        }

        State = State.MoveToRated(rating, extraPoints);

        if (rating is not null)
            Rating = rating;

        if (extraPoints is not null)
            ExtraPoints = extraPoints;
    }

    public void UpdatePoints(Fraction? rating, Points? extraPoints)
    {
        if (rating is null && extraPoints is null)
        {
            const string ratingName = nameof(rating);
            const string extraPointsName = nameof(extraPoints);
            const string message =
                $"Cannot update submission points, both {ratingName} and {extraPointsName} are null.";
            throw new DomainInvalidOperationException(message);
        }

        State = State.MoveToPointsUpdated(rating, extraPoints);

        if (rating is not null)
            Rating = rating;

        if (extraPoints is not null)
            ExtraPoints = extraPoints;
    }

    public void UpdateDate(SpbDateTime newDate)
    {
        State = State.MoveToDateUpdated(newDate);
        SubmissionDate = newDate;
    }

    public void Activate()
    {
        State = State.MoveToActivated();
    }

    public void Deactivate()
    {
        State = State.MoveToDeactivated();
    }

    public void Ban()
    {
        State = State.MoveToBanned();
    }

    public void Delete()
    {
        State = State.MoveToDeleted();
    }

    public void Complete()
    {
        State = State.MoveToCompleted();
    }

    public void MarkAsReviewed()
    {
        State = State.MoveToReviewed();
    }

    private Points? GetEffectivePoints()
    {
        if (Points is null)
            return null;

        Points points = Points.Value;
        DeadlinePenalty? deadlinePolicy = GetEffectiveDeadlinePolicy();

        if (deadlinePolicy is not null)
            points = deadlinePolicy.Apply(points);

        if (ExtraPoints is not null)
            points += ExtraPoints.Value;

        return points;
    }

    private Points? GetPointPenalty()
    {
        if (Points is null)
            return null;

        Points? deadlineAppliedPoints = GetEffectivePoints();

        if (deadlineAppliedPoints is null)
            return null;

        Points? penaltyPoints = Points - deadlineAppliedPoints;

        return penaltyPoints;
    }

    private DeadlinePenalty? GetEffectiveDeadlinePolicy()
    {
        DateOnly deadline = GroupAssignment.Deadline;

        if (SubmissionDateOnly <= deadline)
            return null;

        var submissionDeadlineOffset = TimeSpan.FromDays(SubmissionDateOnly.DayNumber - deadline.DayNumber);

        DeadlinePenalty? activeDeadlinePolicy = GroupAssignment
            .Assignment
            .SubjectCourse
            .DeadlinePolicies
            .Where(dp => dp.SpanBeforeActivation < submissionDeadlineOffset)
            .MaxBy(dp => dp.SpanBeforeActivation);

        return activeDeadlinePolicy;
    }
}