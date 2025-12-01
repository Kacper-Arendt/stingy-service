using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Abstractions;
using Shared.Abstractions.Communication;
using Shared.Abstractions.Events;
using Shared.Abstractions.Factories;
using Shared.Abstractions.Services;
using Shared.Infrastructure.Communication;
using Shared.Infrastructure.Events;
using Shared.Infrastructure.Exceptions;
using Shared.Infrastructure.Factories;
using Shared.Infrastructure.FileService;
using Shared.Infrastructure.Helpers;
using Shared.Infrastructure.Services;

namespace Shared.Infrastructure;

public static class Extensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<ErrorHandlerMiddleware>();
        services.AddSingleton<HttpContextHelper>();
        services.AddSingleton<IDbConnectionStringFactory, DbConnectionStringFactory>();
        services.AddSingleton<IEmailSender, AzureEmailSender>();
        services.AddSingleton<IClock, Clock>();
        
        services.AddSingleton<IEventPublisher, EventPublisher>();
        services.AddHostedService<EventListener>();
        
        services.AddSingleton<IBlobStorageService, AzureBlobStorageService>();
        services.AddSingleton<IAzureOpenAiService, AzureOpenAiService>();

        services.AddSingleton<IFileService, FileService.FileService>();
        
        return services;
    }

    public static WebApplication UseInfrastructure(this WebApplication app)
    {
        app.UseMiddleware<ErrorHandlerMiddleware>();

        return app;
    }
}