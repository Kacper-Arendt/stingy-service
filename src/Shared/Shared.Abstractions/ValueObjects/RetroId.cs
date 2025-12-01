using Shared.Abstractions.Exceptions;
using Shared.Abstractions.ValueObjects;

namespace Retro.Domain.ValueObjects;

public class RetroId : ValueObject
{
    public Guid Value { get; }

    public RetroId(Guid value)
    {
        if (value == Guid.Empty)
            throw new ValueObjectInvalidTypeException("RetroId cannot be empty.");

        Value = value;
    }


    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value.ToString();

    public static implicit operator Guid(RetroId userId) => userId.Value;
    public static explicit operator RetroId(Guid value) => new(value);
}