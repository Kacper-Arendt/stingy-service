namespace Shared.Abstractions.Events;

public interface IEventPublisher
{
    Task PublishAsync<TEvent>(TEvent @event) where TEvent : IEvent;
}