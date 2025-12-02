using Shared.Abstractions.Exceptions;
using Shared.Abstractions.ValueObjects;

namespace UserProfiles.Domain.ValueObjects;

public class ProfileImageUrl : ValueObject
{
    public string? Value { get; }

    public ProfileImageUrl(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            Value = null;
            return;
        }

        if (value.Length > 500)
            throw new ValueObjectInvalidTypeException("ProfileImageUrl cannot exceed 500 characters.");

        if (!IsValidUrl(value))
            throw new ValueObjectInvalidTypeException("ProfileImageUrl must be a valid URL.");

        Value = value;
    }

    private static bool IsValidUrl(string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out var result) 
               && (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps);
    }

    public static implicit operator string?(ProfileImageUrl? profileImageUrl) => profileImageUrl?.Value;
    public static explicit operator ProfileImageUrl(string? value) => new(value);

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}
