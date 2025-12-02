using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Shared.Infrastructure.Helpers;
using UserProfiles.Core.Commands;
using UserProfiles.Core.Commands.Dtos;
using UserProfiles.Core.Queries;

namespace UserProfiles.Api.Endpoints;

public static class ProductTourEndpoints
{
    public static RouteGroupBuilder MapProductTourEndpoints(this RouteGroupBuilder group)
    {
        group.MapPost("me/tours", async (
            SetTourStatusDto dto,
            HttpContextHelper ctx,
            IProductTourCommandService productTourCommandService
        ) =>
        {
            var currentUser = ctx.GetCurrentUser();
            await productTourCommandService.SetTourStatusAsync(currentUser.Id, dto);
            return Results.Ok(new { Message = "Tour status updated successfully" });
        })
        .RequireAuthorization();

        group.MapGet("me/tours", async (
            HttpContextHelper ctx,
            IProductTourQueryService productTourQueryService
        ) =>
        {
            var currentUser = ctx.GetCurrentUser();
            var result = await productTourQueryService.GetAllTourStatusesAsync(currentUser.Id);
            return Results.Ok(result);
        })
        .RequireAuthorization();

        return group;
    }
}