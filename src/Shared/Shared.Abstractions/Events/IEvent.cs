namespace Shared.Abstractions.Events;

public interface IEvent
{
    public DateTime OccurredOn { get; }
};