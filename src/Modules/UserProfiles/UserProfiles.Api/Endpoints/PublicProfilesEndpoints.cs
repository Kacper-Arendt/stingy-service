using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Authorization;
using Shared.Abstractions.ValueObjects;
using Shared.Infrastructure.Helpers;
using UserProfiles.Core.Queries;

namespace UserProfiles.Api.Endpoints;

public static class PublicProfilesEndpoints
{
    public static RouteGroupBuilder MapPublicProfilesEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("{userId}/public", async (
            Guid userId,
            IUserProfileQueryService queryService,
            HttpContextHelper httpContextHelper) =>
        {
            var currentUser = httpContextHelper.GetCurrentUser();
            var result = await queryService.GetPublicProfileAsync(new UserId(userId), currentUser.Id);
            return result == null ? Results.NotFound("Profile not found or not accessible.") : TypedResults.Ok(result);
        });

        return group;
    }
}
