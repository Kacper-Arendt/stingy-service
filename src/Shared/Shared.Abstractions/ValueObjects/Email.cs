using Shared.Abstractions.Exceptions;

namespace Shared.Abstractions.ValueObjects;

public class Email : ValueObject
{
    public string Value { get; }

    public Email(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ValueObjectInvalidTypeException("Email cannot be empty.");
        
        if (value.Length > 100)
            throw new ValueObjectInvalidTypeException("Email cannot exceed 100 characters.");

        Value = value;
    }
    
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value.ToString();

    public static implicit operator string(Email userId) => userId.Value;
    public static explicit operator Email(string value) => new(value);
}