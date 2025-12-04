using Teams.Core.Commands;
using Teams.Core.Queries;
using Teams.Core.Repositories;
using Teams.Core.Permissions;
using Teams.Core.Authorization;
using Teams.Core.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authorization;
using Shared.Abstractions.Events;

namespace Teams.Core;

public static class Extensions
{
    public static IServiceCollection AddTeamsCore(this IServiceCollection services)
    {
        services.AddScoped<ITeamRepository, TeamRepository>();
        services.AddScoped<ITeamCommandService, TeamCommandService>();
        services.AddScoped<ITeamQueryService, TeamQueryService>();
        services.AddScoped<ITeamPermissionService, TeamPermissionService>();
        
        services.AddScoped<IAuthorizationHandler, HasAccessToTeamHandler>();
        services.AddScoped<IEventHandler<TeamParticipantInvitedEvent>, TeamParticipantInvitedEventHandler>();

        return services;
    }
} 