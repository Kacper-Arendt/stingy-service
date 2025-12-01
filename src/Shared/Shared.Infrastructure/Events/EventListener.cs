using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Shared.Abstractions.Events;

namespace Shared.Infrastructure.Events;

public class EventListener : BackgroundService
{
    private readonly ServiceBusProcessor _processor;
    private readonly ILogger<EventListener> _logger;
    private readonly IServiceProvider _serviceProvider;

    public EventListener(IConfiguration configuration, ILogger<EventListener> logger, IServiceProvider serviceProvider)
    {
        var client = new ServiceBusClient(configuration["Bus:ConnectionString"]);
        _processor = client.CreateProcessor("default");
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _processor.ProcessMessageAsync += MessageHandler;
        _processor.ProcessErrorAsync += ErrorHandler;

        await _processor.StartProcessingAsync(stoppingToken);
    }

    private async Task MessageHandler(ProcessMessageEventArgs args)
    {
        if (!args.Message.ApplicationProperties.TryGetValue("EventType", out var eventTypeObj))
        {
            _logger.LogWarning("EventType property missing in message.");
            await args.CompleteMessageAsync(args.Message);
            return;
        }

        var eventTypeName = eventTypeObj.ToString();
        var eventType = Type.GetType(eventTypeName);
        if (eventType == null)
        {
            _logger.LogWarning("Unknown event type: {EventType}", eventTypeName);
            await args.CompleteMessageAsync(args.Message);
            return;
        }

        var eventData = JsonSerializer.Deserialize(args.Message.Body.ToString(), eventType);
        if (eventData == null)
        {
            _logger.LogWarning("Failed to deserialize event: {EventType}", eventTypeName);
            await args.CompleteMessageAsync(args.Message);
            return;
        }

        using var scope = _serviceProvider.CreateScope();
        var handlerType = typeof(IEventHandler<>).MakeGenericType(eventType);
        var handlers = scope.ServiceProvider.GetServices(handlerType);

        foreach (var handler in handlers)
        {
            var handleMethod = handlerType.GetMethod("HandleAsync");
            if (handleMethod == null) continue;
            
            var task = (Task)handleMethod.Invoke(handler, new[] { eventData });
            await task;
        }

        await args.CompleteMessageAsync(args.Message);
    }
    
    private Task ErrorHandler(ProcessErrorEventArgs args)
    {
        _logger.LogError(args.Exception, "Error processing message");
        return Task.CompletedTask;
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await _processor.StopProcessingAsync(cancellationToken);
        await _processor.DisposeAsync();
    }
}