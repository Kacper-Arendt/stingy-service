using Shared.Abstractions.Exceptions;

namespace Shared.Abstractions.ValueObjects;

public class DateTimeZone : ValueObject
{
    public string Value { get; }
    
    public const string DefaultTimeZone = "Europe/Warsaw";

    public DateTimeZone(string? value = null)
    {
        var timeZoneValue = string.IsNullOrWhiteSpace(value) ? DefaultTimeZone : value;

        if (!IsValidTimeZone(timeZoneValue))
            throw new ValueObjectInvalidTypeException($"TimeZone '{timeZoneValue}' is not valid.");

        Value = timeZoneValue;
    }

    private static bool IsValidTimeZone(string timeZone)
    {
        try
        {
            TimeZoneInfo.FindSystemTimeZoneById(timeZone);
            return true;
        }
        catch (TimeZoneNotFoundException)
        {
            try
            {
                var timeZoneInfo = TimeZoneInfo.GetSystemTimeZones()
                    .FirstOrDefault(tz => tz.Id.Equals(timeZone, StringComparison.OrdinalIgnoreCase) ||
                                         tz.StandardName.Equals(timeZone, StringComparison.OrdinalIgnoreCase));
                return timeZoneInfo != null;
            }
            catch
            {
                return false;
            }
        }
        catch
        {
            return false;
        }
    }

    public static implicit operator string(DateTimeZone dateTimeZone) => dateTimeZone.Value;
    public static explicit operator DateTimeZone(string value) => new(value);

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}
