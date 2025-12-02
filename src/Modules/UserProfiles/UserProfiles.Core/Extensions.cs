using Microsoft.Extensions.DependencyInjection;
using UserProfiles.Core.Commands;
using UserProfiles.Core.Queries;
using UserProfiles.Core.Services;
using UserProfiles.Core.Permissions;
using UserProfiles.Core.Repositories;
using UserProfiles.Core.Events;
using UserProfiles.Shared.Services;
using Shared.Abstractions.Events;

namespace UserProfiles.Core;

public static class Extensions
{
    public static IServiceCollection AddUserProfilesCore(this IServiceCollection services)
    {
        // Command Services
        services.AddScoped<IUserProfileCommandService, UserProfileCommandService>();
        services.AddScoped<IProductTourCommandService, ProductTourCommandService>();

        // Query Services
        services.AddScoped<IUserProfileQueryService, UserProfileQueryService>();
        services.AddScoped<IProductTourQueryService, ProductTourQueryService>();

        // Domain Services
        services.AddScoped<IProfileImageService, ProfileImageService>();
        services.AddScoped<IUserDisplayService, UserDisplayService>();

        // Permission Services
        services.AddScoped<IUserProfilePermissionService, UserProfilePermissionService>();

        // Repositories
        services.AddScoped<IUserProfileRepository, UserProfileRepository>();
        services.AddScoped<IProductTourPostStatusRepository, ProductTourPostStatusRepository>();

        // Event Handlers
        services.AddScoped<IEventHandler<UserRegisteredEvent>, UserRegisteredEventHandler>();

        return services;
    }
}
