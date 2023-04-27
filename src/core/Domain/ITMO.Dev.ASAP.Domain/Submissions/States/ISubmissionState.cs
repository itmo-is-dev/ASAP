using ITMO.Dev.ASAP.Domain.Models;
using ITMO.Dev.ASAP.Domain.Tools;
using ITMO.Dev.ASAP.Domain.ValueObject;

namespace ITMO.Dev.ASAP.Domain.Submissions.States;

public interface ISubmissionState
{
    SubmissionStateKind Kind { get; }

    /// <summary>
    ///     Gets a value indicating whether the state is terminal, yet relevant for the system.
    /// </summary>
    bool IsTerminalEffectiveState { get; }

    ISubmissionState MoveToRated(Fraction? rating, Points? extraPoints);

    ISubmissionState MoveToPointsUpdated(Fraction? rating, Points? extraPoints);

    ISubmissionState MoveToBanned();

    ISubmissionState MoveToActivated();

    ISubmissionState MoveToDeactivated();

    ISubmissionState MoveToDateUpdated(SpbDateTime newDate);

    ISubmissionState MoveToDeleted();

    ISubmissionState MoveToCompleted();

    ISubmissionState MoveToReviewed();
}