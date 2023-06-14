using ITMO.Dev.ASAP.DataAccess.Models.DeadlinePenalties;
using ITMO.Dev.ASAP.Domain.Deadlines.DeadlinePenalties;

namespace ITMO.Dev.ASAP.DataAccess.Mapping;

public static class DeadlinePenaltyMapper
{
    public static DeadlinePenalty MapTo(DeadlinePenaltyModel model)
    {
        return model switch
        {
            AbsoluteDeadlinePenaltyModel penalty
                => new AbsoluteDeadlinePenalty(penalty.SpanBeforeActivation, penalty.AbsoluteValue),

            CappingDeadlinePenaltyModel penalty
                => new CappingDeadlinePenalty(penalty.SpanBeforeActivation, penalty.Cap),

            FractionDeadlinePenaltyModel penalty
                => new FractionDeadlinePenalty(penalty.SpanBeforeActivation, penalty.Fraction),

            _ => throw new ArgumentOutOfRangeException(nameof(model)),
        };
    }

    public static DeadlinePenaltyModel MapFrom(DeadlinePenalty entity, Guid? id, Guid subjectCourseId)
    {
        return entity switch
        {
            AbsoluteDeadlinePenalty penalty => new AbsoluteDeadlinePenaltyModel(
                id ?? Guid.NewGuid(),
                subjectCourseId,
                penalty.SpanBeforeActivation,
                penalty.AbsoluteValue.Value),

            CappingDeadlinePenalty penalty => new CappingDeadlinePenaltyModel(
                id ?? Guid.NewGuid(),
                subjectCourseId,
                penalty.SpanBeforeActivation,
                penalty.Cap.Value),

            FractionDeadlinePenalty penalty => new FractionDeadlinePenaltyModel(
                id ?? Guid.NewGuid(),
                subjectCourseId,
                penalty.SpanBeforeActivation,
                penalty.Fraction.Value),

            _ => throw new ArgumentOutOfRangeException(nameof(entity)),
        };
    }
}