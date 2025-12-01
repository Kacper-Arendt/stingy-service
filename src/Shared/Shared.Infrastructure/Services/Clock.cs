using Shared.Abstractions.Services;

namespace Shared.Infrastructure.Services;

public class Clock : IClock
{
    public DateTime UtcNow => DateTime.UtcNow;
    public DateTime Today => DateTime.UtcNow.Date;
}