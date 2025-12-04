using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Teams.Shared.Services;

namespace Teams.Shared;

public static class Extensions
{
    public static WebApplicationBuilder RegisterTeamsSharedModule(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<ITeamMembershipQueryService, TeamMembershipQueryService>();

        return builder;
    }
}