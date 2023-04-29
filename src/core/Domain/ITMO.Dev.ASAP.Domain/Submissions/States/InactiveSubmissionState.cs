using ITMO.Dev.ASAP.Common.Exceptions;
using ITMO.Dev.ASAP.Domain.Models;
using ITMO.Dev.ASAP.Domain.Tools;
using ITMO.Dev.ASAP.Domain.ValueObject;

namespace ITMO.Dev.ASAP.Domain.Submissions.States;

public class InactiveSubmissionState : ISubmissionState
{
    public SubmissionStateKind Kind => SubmissionStateKind.Inactive;

    public bool IsTerminalEffectiveState => false;

    public ISubmissionState MoveToRated(Fraction? rating, Points? extraPoints)
    {
        const string message = "Submission is inactive and cannot be rated";
        throw new DomainInvalidOperationException(message);
    }

    public ISubmissionState MoveToPointsUpdated(Fraction? rating, Points? extraPoints)
    {
        const string message = "Cannot update points of inactive submission";
        throw new DomainInvalidOperationException(message);
    }

    public ISubmissionState MoveToBanned()
    {
        return new BannedSubmissionState();
    }

    public ISubmissionState MoveToActivated()
    {
        return new ActiveSubmissionState();
    }

    public ISubmissionState MoveToDeactivated()
    {
        const string message = "Submission is already inactive";
        throw new DomainInvalidOperationException(message);
    }

    public ISubmissionState MoveToDateUpdated(SpbDateTime newDate)
    {
        const string message = "Cannot update date of inactive submission";
        throw new DomainInvalidOperationException(message);
    }

    public ISubmissionState MoveToDeleted()
    {
        return new DeletedSubmissionState();
    }

    public ISubmissionState MoveToCompleted()
    {
        const string message = "Cannot complete inactive submission";
        throw new DomainInvalidOperationException(message);
    }

    public ISubmissionState MoveToReviewed()
    {
        const string message = "Cannot review inactive submission";
        throw new DomainInvalidOperationException(message);
    }
}