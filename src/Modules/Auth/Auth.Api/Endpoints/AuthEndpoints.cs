using Auth.Core.Dtos;
using Auth.Core.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Auth.Api.Endpoints;

public static class AuthEndpoints
{
    public static RouteGroupBuilder MapAuthEndpoints(this RouteGroupBuilder group)
    {
        group.MapPost("register", async (
            RegisterUserDto registerUserDto,
            IAuthService authService) =>
        {
            var createdUser = await authService.RegisterUser(registerUserDto);
            return TypedResults.Ok(new { Id = createdUser.UserId });
        });

        group.MapGet("me", async (IAuthService authService) =>
        {
            var user = await authService.GetCurrentUser();
            return TypedResults.Ok(user);
        })
        .RequireAuthorization();

        group.MapPost("logout", async (IAuthService authService) =>
        {
            await authService.Logout();
            return TypedResults.NoContent();
        })
        .RequireAuthorization();

        return group;
    }
}

