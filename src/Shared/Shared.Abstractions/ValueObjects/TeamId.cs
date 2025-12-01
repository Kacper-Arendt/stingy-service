using Shared.Abstractions.Exceptions;

namespace Shared.Abstractions.ValueObjects;

public class TeamId : ValueObject
{
    public Guid Value { get; }

    public TeamId(Guid value)
    {
        if (value == Guid.Empty)
            throw new ValueObjectInvalidTypeException("TeamId cannot be empty.");

        Value = value;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value.ToString();

    public static implicit operator Guid(TeamId teamId) => teamId.Value;
    public static explicit operator TeamId(Guid value) => new(value);
} 