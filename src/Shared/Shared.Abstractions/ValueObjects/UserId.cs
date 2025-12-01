using Shared.Abstractions.Exceptions;

namespace Shared.Abstractions.ValueObjects;

public class UserId : ValueObject
{
    public Guid Value { get; }

    public UserId(Guid value)
    {
        if (value == Guid.Empty)
            throw new ValueObjectInvalidTypeException("UserId cannot be empty.");

        Value = value;
    }


    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value.ToString();

    public static implicit operator Guid(UserId userId) => userId.Value;
    public static explicit operator UserId(Guid value) => new(value);
}