using Shared.Abstractions.Exceptions;
using Shared.Abstractions.ValueObjects;

namespace UserProfiles.Domain.ValueObjects;

public class TourKey : ValueObject
{
    public string Value { get; }

    public TourKey(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ValueObjectInvalidTypeException("TourKey cannot be empty.");

        if (value.Length > 100)
            throw new ValueObjectInvalidTypeException("TourKey cannot exceed 100 characters.");

        Value = value.Trim();
    }

    public static implicit operator string(TourKey tourKey) => tourKey.Value;
    public static explicit operator TourKey(string value) => new(value);

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}

