using Shared.Abstractions.ValueObjects;

namespace Budgets.Domain.ValueObjects;

public class YearValue : ValueObject
{
    public int Value { get; }

    public YearValue(int value)
    {
        Value = value;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value.ToString();

    public static implicit operator int(YearValue yearValue) => yearValue.Value;
    public static explicit operator YearValue(int value) => new(value);
}
