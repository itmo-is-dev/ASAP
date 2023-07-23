using ITMO.Dev.ASAP.Common.Exceptions;
using ITMO.Dev.ASAP.Domain.Deadlines.DeadlinePolicies;
using ITMO.Dev.ASAP.Domain.Students;
using ITMO.Dev.ASAP.Domain.Study.Assignments;
using ITMO.Dev.ASAP.Domain.Study.GroupAssignments;
using ITMO.Dev.ASAP.Domain.Submissions.States;
using ITMO.Dev.ASAP.Domain.Tools;
using ITMO.Dev.ASAP.Domain.ValueObject;
using RichEntity.Annotations;

namespace ITMO.Dev.ASAP.Domain.Submissions;

public partial class Submission : IEntity<Guid>
{
    public Submission(
        Guid id,
        int code,
        Student student,
        SpbDateTime submissionDate,
        string payload,
        GroupAssignment groupAssignment,
        ISubmissionState state)
        : this(id)
    {
        Code = code;
        SubmissionDate = submissionDate;
        Student = student;
        Payload = payload;

        GroupAssignment = groupAssignment;

        Rating = default;
        ExtraPoints = default;

        State = state;
    }

    public Submission(
        Guid id,
        int code,
        Student student,
        SpbDateTime submissionDate,
        string payload,
        GroupAssignment groupAssignment)
        : this(
            id,
            code,
            student,
            submissionDate,
            payload,
            groupAssignment,
            new ActiveSubmissionState()) { }

    public int Code { get; }

    public string Payload { get; }

    public Fraction? Rating { get; private set; }

    public Points? ExtraPoints { get; private set; }

    public SpbDateTime SubmissionDate { get; private set; }

    public Student Student { get; }

    public GroupAssignment GroupAssignment { get; }

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

    public RatedSubmission CalculateEffectivePoints(Assignment assignment, DeadlinePolicy policy)
    {
        Points points = assignment.MaxPoints * (Rating ?? Fraction.None);

        Points? penalty = policy.GetPointPenalty(points, GroupAssignment.Deadline, SubmissionDateOnly);

        if (penalty is not null)
        {
            points -= penalty.Value;
        }

        points += ExtraPoints ?? Points.None;

        return new RatedSubmission(this, points);
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
}