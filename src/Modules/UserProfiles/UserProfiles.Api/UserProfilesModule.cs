using Microsoft.AspNetCore.Builder;
using UserProfiles.Api.Endpoints;
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
        var profilesGroup = app.MapGroup("api/profiles");
        profilesGroup.MapProductTourEndpoints();
        profilesGroup.MapProfileImageEndpoints();
        return app;
    }
}
