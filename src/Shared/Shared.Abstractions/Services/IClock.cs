namespace Shared.Abstractions.Services;

public interface IClock
{
    DateTime UtcNow { get; }
    DateTime Today { get; }
}