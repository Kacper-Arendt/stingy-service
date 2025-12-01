using Shared.Abstractions.Exceptions;

namespace Shared.Abstractions.ValueObjects;

public class CreatedAtDate : ValueObject
{
    public DateTime Value { get; }

    public CreatedAtDate(DateTime value)
    {
        if (value == default)
            throw new ValueObjectInvalidTypeException("CreatedAtDate cannot be default value.");

        if (value > DateTime.UtcNow.AddMinutes(5)) // Tolerancja 5 minut na różnice w zegarach
            throw new ValueObjectInvalidTypeException("CreatedAtDate cannot be in the future.");

        Value = value.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(value, DateTimeKind.Utc) : value.ToUniversalTime();
    }

    public static implicit operator DateTime(CreatedAtDate date) => date.Value;
    public static explicit operator CreatedAtDate(DateTime value) => new(value);

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}