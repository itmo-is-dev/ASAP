namespace ITMO.Dev.ASAP.DataAccess.Models.DeadlinePenalties;

public partial class FractionDeadlinePenaltyModel : DeadlinePenaltyModel
{
    public FractionDeadlinePenaltyModel(
        Guid id,
        Guid subjectCourseId,
        TimeSpan spanBeforeActivation,
        double fraction) : base(id, subjectCourseId, spanBeforeActivation)
    {
        Fraction = fraction;
    }

    public double Fraction { get; set; }
}