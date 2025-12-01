using Shared.Abstractions.Exceptions;

namespace Shared.Abstractions.ValueObjects;

public class LastModifiedDate : ValueObject
{
    public DateTime Value { get; }

    public LastModifiedDate(DateTime value)
    {
        if (value == default)
            throw new ValueObjectInvalidTypeException("LastModifiedDate cannot be default value.");

        if (value > DateTime.UtcNow.AddMinutes(5))
            throw new ValueObjectInvalidTypeException("LastModifiedDate cannot be in the future.");

        Value = value.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(value, DateTimeKind.Utc) : value.ToUniversalTime();
    }

    public static implicit operator DateTime(LastModifiedDate lastModified) => lastModified.Value;
    public static explicit operator LastModifiedDate(DateTime value) => new(value);

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}
