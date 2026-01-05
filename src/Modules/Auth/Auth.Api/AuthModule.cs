using Auth.Api.Endpoints;
using Auth.Core;
using Auth.Core.Database;
using Auth.Core.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Auth.Api;

public static class AuthModule
{
    public static WebApplicationBuilder RegisterAuthModule(this WebApplicationBuilder builder)
    {
        builder.Services.AddAuthCore(builder.Configuration);

        builder.Services.AddIdentityCore<User>()
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<UsersEfContext>()
            .AddApiEndpoints();

        builder.Services.AddIdentityApiEndpoints<User>(options =>
            {
                options.SignIn.RequireConfirmedEmail = false;
                options.User.RequireUniqueEmail = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireDigit = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromHours(2);
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<UsersEfContext>();

        builder.Services.ConfigureApplicationCookie(options =>
        {
            options.Cookie.Name = ".AspNetCore.Identity.Application";
            options.Cookie.HttpOnly = true;
            options.SlidingExpiration = true;

            options.ExpireTimeSpan = builder.Environment.IsDevelopment() ? TimeSpan.FromMinutes(20) : TimeSpan.FromDays(35);
        });

        return builder;
    }

    public static WebApplication UseAuthModule(this WebApplication app)
    {
        app
            .MapGroup("api/auth/identity")
            .MapIdentityApi<User>()
            .WithTags("Auth");

        app
            .MapGroup("api/auth")
            .MapAuthEndpoints()
            .WithTags("Auth");

        return app;
    }
}