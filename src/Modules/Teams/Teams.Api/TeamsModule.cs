using Teams.Core;
using Teams.Core.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Teams.Api.Endpoints;

namespace Teams.Api;

public static class TeamsModule
{
    public static WebApplicationBuilder RegisterTeamsModule(this WebApplicationBuilder builder)
    {
        builder.Services.AddTeamsCore();
        
        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy(Policies.HasAccessToTeam, policy =>
                policy.Requirements.Add(new HasAccessToTeam()));
            
            options.AddPolicy(Policies.CanManageTeam, policy =>
                policy.Requirements.Add(new HasAccessToTeam()));
                
            options.AddPolicy(Policies.CanManageTeamParticipants, policy =>
                policy.Requirements.Add(new HasAccessToTeam()));
        });

        return builder;
    }

    public static WebApplication UseTeamsModule(this WebApplication app)
    {
        app.MapGroup("/api/teams")
            .RequireAuthorization()
            .MapTeamsEndpoints()
            .MapTeamEndpoints()
            .WithTags("Teams");
        
        return app;
    }
} 