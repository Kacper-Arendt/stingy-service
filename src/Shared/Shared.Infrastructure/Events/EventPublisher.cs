using Azure.Messaging.ServiceBus;
using Shared.Abstractions.Events;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace Shared.Infrastructure.Events
{
    public class EventPublisher : IEventPublisher
    {
        private readonly ServiceBusClient _client;
        private readonly ServiceBusSender _sender;

        public EventPublisher(IConfiguration configuration)
        {
            var connectionString = configuration["Bus:ConnectionString"];
            _client = new ServiceBusClient(connectionString);
            _sender = _client.CreateSender("default");
        }

        public async Task PublishAsync<TEvent>(TEvent @event) where TEvent : IEvent
        {
            var message = new ServiceBusMessage(JsonSerializer.Serialize(@event))
            {
                ApplicationProperties = { ["EventType"] = @event.GetType().AssemblyQualifiedName }
            };
            
            await _sender.SendMessageAsync(message);
        }
    }
}