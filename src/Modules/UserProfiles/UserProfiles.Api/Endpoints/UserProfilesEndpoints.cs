using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Shared.Infrastructure.Helpers;
using UserProfiles.Core.Commands;
using UserProfiles.Core.Commands.Dtos;
using UserProfiles.Core.Queries;

namespace UserProfiles.Api.Endpoints;

public static class UserProfilesEndpoints
{
    public static RouteGroupBuilder MapUserProfilesEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("me", async (HttpContextHelper httpContextHelper, IUserProfileQueryService queryService) =>
        {
            var currentUser = httpContextHelper.GetCurrentUser();
            var result = await queryService.GetByUserIdAsync(currentUser.Id);
            return result == null ? Results.NotFound("User profile not found.") : TypedResults.Ok(result);
        });

        group.MapPut("me", async (HttpContextHelper httpContextHelper, IUserProfileCommandService commandService, UpdateUserProfileDto dto) =>
        {
            var currentUser = httpContextHelper.GetCurrentUser();
            var result = await commandService.UpdateAsync(currentUser.Id, dto);
            return TypedResults.Ok(result);
        });

        group.MapPost("me", async (HttpContextHelper httpContextHelper, IUserProfileCommandService commandService, CreateUserProfileDto dto) =>
        {
            var currentUser = httpContextHelper.GetCurrentUser();
            var result = await commandService.CreateAsync(currentUser.Id, dto);
            return TypedResults.Ok(result);
        });

        return group;
    }
}
