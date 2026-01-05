using Auth.Api;
using Shared.Infrastructure;
using Teams.Api;
using Teams.Shared;
using UserProfiles.Api;

namespace Stingy.App.DI;

public static class ModuleLoader
{
    public static WebApplicationBuilder RegisterModules(this WebApplicationBuilder builder)
    {
        builder.Services.AddInfrastructure(builder.Configuration);
        builder.RegisterAuthModule();
        builder.RegisterTeamsSharedModule();
        builder.RegisterTeamsModule();
        builder.RegisterUserProfilesModule();

        return builder;
    }

    public static WebApplication UseModules(this WebApplication app)
    {
        app.UseInfrastructure();
        app.UseAuthModule();
        app.UseTeamsModule();
        app.UseUserProfilesModule();

        return app;
    }
}