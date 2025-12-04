using Shared.Abstractions.Exceptions;
using Shared.Abstractions.ValueObjects;

namespace Teams.Domain.ValueObjects;

public class TeamDescription : ValueObject
{
    public string Value { get; }

    public TeamDescription(string value)
    {
        if (value != null && value.Length > 500)
            throw new ValueObjectInvalidTypeException("TeamDescription cannot exceed 500 characters.");

        Value = value ?? string.Empty;
    }

    public static implicit operator string(TeamDescription teamDescription) => teamDescription.Value;
    public static explicit operator TeamDescription(string value) => new(value);

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
} 