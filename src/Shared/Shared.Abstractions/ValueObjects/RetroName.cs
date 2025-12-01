using Shared.Abstractions.Exceptions;
using Shared.Abstractions.ValueObjects;

namespace Retro.Domain.ValueObjects;

public class RetroName : ValueObject
{
    public string Value { get; }

    public RetroName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ValueObjectInvalidTypeException("RetroName cannot be empty.");

        if (value.Length > 100)
            throw new ValueObjectInvalidTypeException("Team cannot exceed 100 characters.");

        Value = value;
    }

    public static implicit operator string(RetroName retroName) => retroName.Value;
    public static explicit operator RetroName(string value) => new(value);

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}