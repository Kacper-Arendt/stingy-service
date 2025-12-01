using Shared.Abstractions.ValueObjects;

namespace Shared.Abstractions.Events;

public class UserRegisteredEvent : IEvent
{
    public UserId UserId { get; set; } = null!;
    public Email Email { get; set; } = null!;
    public DateTime RegisteredAt { get; set; }
    public DateTime OccurredOn { get; }
}
