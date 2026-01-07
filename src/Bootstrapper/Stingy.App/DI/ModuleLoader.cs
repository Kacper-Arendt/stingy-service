using Auth.Api;
using Budgets.Api;
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
        builder.RegisterBudgetsModule();

        return builder;
    }

    public static WebApplication UseModules(this WebApplication app)
    {
        app.UseInfrastructure();
        app.UseAuthModule();
        app.UseTeamsModule();
        app.UseUserProfilesModule();
        app.UseBudgetsModule();

        return app;
    }
}