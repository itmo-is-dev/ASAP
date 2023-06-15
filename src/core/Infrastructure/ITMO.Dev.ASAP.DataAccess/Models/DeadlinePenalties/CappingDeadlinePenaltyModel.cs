namespace ITMO.Dev.ASAP.DataAccess.Models.DeadlinePenalties;

public partial class CappingDeadlinePenaltyModel : DeadlinePenaltyModel
{
    public CappingDeadlinePenaltyModel(
        Guid id,
        Guid subjectCourseId,
        TimeSpan spanBeforeActivation,
        double cap) : base(id, subjectCourseId, spanBeforeActivation)
    {
        Cap = cap;
    }

    public double Cap { get; set; }
}