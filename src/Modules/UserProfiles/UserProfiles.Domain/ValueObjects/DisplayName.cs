using Shared.Abstractions.Exceptions;
using Shared.Abstractions.ValueObjects;

namespace UserProfiles.Domain.ValueObjects;

public class DisplayName : ValueObject
{
    public string Value { get; }

    public DisplayName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ValueObjectInvalidTypeException("DisplayName cannot be empty.");

        if (value.Length < 2)
            throw new ValueObjectInvalidTypeException("DisplayName must be at least 2 characters long.");

        if (value.Length > 50)
            throw new ValueObjectInvalidTypeException("DisplayName cannot exceed 50 characters.");
        
        Value = value.Trim();
    }

    public static implicit operator string(DisplayName displayName) => displayName.Value;
    public static explicit operator DisplayName(string value) => new(value);

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}
