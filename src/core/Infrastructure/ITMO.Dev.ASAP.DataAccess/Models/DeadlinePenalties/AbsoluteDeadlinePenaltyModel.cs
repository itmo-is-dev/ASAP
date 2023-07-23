namespace ITMO.Dev.ASAP.DataAccess.Models.DeadlinePenalties;

public partial class AbsoluteDeadlinePenaltyModel : DeadlinePenaltyModel
{
    public AbsoluteDeadlinePenaltyModel(
        Guid id,
        Guid subjectCourseId,
        TimeSpan spanBeforeActivation,
        double absoluteValue) : base(id, subjectCourseId, spanBeforeActivation)
    {
        AbsoluteValue = absoluteValue;
    }

    public double AbsoluteValue { get; set; }
}