using Shared.Abstractions.Exceptions;
using Shared.Abstractions.ValueObjects;

namespace UserProfiles.Domain.ValueObjects;

public class UserBio : ValueObject
{
    public string? Value { get; }

    public UserBio(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            Value = null;
            return;
        }

        if (value.Length > 500)
            throw new ValueObjectInvalidTypeException("UserBio cannot exceed 500 characters.");

        Value = value.Trim();
    }

    public static implicit operator string?(UserBio? userBio) => userBio?.Value;
    public static explicit operator UserBio(string? value) => new(value);

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}
