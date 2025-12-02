using Microsoft.AspNetCore.Builder;
using UserProfiles.Core;

namespace UserProfiles.Api;

public static class UserProfilesModule
{
    public static WebApplicationBuilder RegisterUserProfilesModule(this WebApplicationBuilder builder)
    {
        builder.Services.AddUserProfilesCore();

        return builder;
    }

    public static WebApplication UseUserProfilesModule(this WebApplication app)
    {
        return app;
    }
}
