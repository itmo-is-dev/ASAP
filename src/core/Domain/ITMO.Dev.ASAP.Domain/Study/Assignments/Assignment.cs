using ITMO.Dev.ASAP.Common.Exceptions;
using ITMO.Dev.ASAP.Domain.ValueObject;
using RichEntity.Annotations;

namespace ITMO.Dev.ASAP.Domain.Study.Assignments;

public partial class Assignment : IEntity<Guid>
{
    public Assignment(
        Guid id,
        string title,
        string shortName,
        int order,
        Points minPoints,
        Points maxPoints)
        : this(id)
    {
        if (minPoints > maxPoints)
            throw new ArgumentException("minPoints must be less than or equal to maxPoints");

        Title = title;
        ShortName = shortName;
        Order = order;
        MinPoints = minPoints;
        MaxPoints = maxPoints;
    }

    public string Title { get; set; }

    public string ShortName { get; set; }

    public int Order { get; set; }

    public Points MinPoints { get; protected set; }

    public Points MaxPoints { get; protected set; }

    public AssignmentInfo Info => new AssignmentInfo(Id, Title, ShortName);

    public void UpdateMinPoints(Points value)
    {
        if (value > MaxPoints)
        {
            string message = $"New minimal points ({value}) are greater than maximal points ({MaxPoints})";
            throw new DomainInvalidOperationException(message);
        }

        MinPoints = value;
    }

    public void UpdateMaxPoints(Points value)
    {
        if (value < MinPoints)
        {
            string message = $"New maximal points ({value}) are less than minimal points ({MinPoints})";
            throw new DomainInvalidOperationException(message);
        }

        MaxPoints = value;
    }

    public Points RatedWith(Fraction? value)
    {
        return MaxPoints * value ?? Points.None;
    }

    public override string ToString()
    {
        return $"Id: {Id}, Title: {ShortName}";
    }
}