namespace Kysect.Shreks.Core.ValueObject;

public readonly record struct Fraction
{
    public Fraction(double value)
    {
        if (value is < 0 or > 1)
            throw new ArgumentOutOfRangeException(nameof(value), "Value must be between 0 and 1");
        
        Value = value;
    }

    public double Value { get; }
    
    public static implicit operator Fraction(double value)
        => new Fraction(value);

    public static implicit operator double(Fraction fraction)
        => fraction.Value;
}