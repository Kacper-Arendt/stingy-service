using Auth.Api;
using Shared.Infrastructure;
using UserProfiles.Api;

namespace Stingy.App.DI;

public static class ModuleLoader
{
    public static WebApplicationBuilder RegisterModules(this WebApplicationBuilder builder)
    {
        builder.Services.AddInfrastructure(builder.Configuration);
        builder.RegisterAuthModule();
        // builder.RegisterRetroModule();
        // builder.RegisterTeamsSharedModule();
        // builder.RegisterTeamsModule();
        builder.RegisterUserProfilesModule();

        // builder.Services.AddControllers()
            // .AddApplicationPart(typeof(PermissionController).Assembly)
            // .AddApplicationPart(typeof(TeamsController).Assembly)
            // .AddApplicationPart(typeof(TeamRetroController).Assembly)
            // .AddApplicationPart(typeof(UserProfilesController).Assembly);

        return builder;
    }

    public static WebApplication UseModules(this WebApplication app)
    {
        app.UseInfrastructure();
        app.UseAuthModule();
        // app.UseRetroModule();
        // app.UseTeamsModule();
        app.UseUserProfilesModule();

        return app;
    }
}