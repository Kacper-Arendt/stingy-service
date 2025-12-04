using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
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
        app.MapGroup("api/profiles")
            .MapUserProfilesEndpoints()
            .MapProductTourEndpoints()
            .MapProfileImageEndpoints()
            .MapPublicProfilesEndpoints()
            .RequireAuthorization()
            .WithTags("UserProfiles");
        
        return app;
    }
}
