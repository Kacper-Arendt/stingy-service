using Shared.Abstractions.Exceptions;
using Shared.Abstractions.ValueObjects;

namespace Budgets.Domain.ValueObjects;

public class YearId : ValueObject
{
    public Guid Value { get; }

    public YearId(Guid value)
    {
        if (value == Guid.Empty)
            throw new ValueObjectInvalidTypeException("YearId cannot be empty.");

        Value = value;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value.ToString();

    public static implicit operator Guid(YearId yearId) => yearId.Value;
    public static explicit operator YearId(Guid value) => new(value);
}
