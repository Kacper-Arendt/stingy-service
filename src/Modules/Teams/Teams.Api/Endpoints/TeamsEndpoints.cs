using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Shared.Abstractions.ValueObjects;
using Teams.Core.Commands;
using Teams.Core.Commands.Dtos;
using Teams.Core.Queries;

namespace Teams.Api.Endpoints;

public static class TeamsEndpoints
{
    public static RouteGroupBuilder MapTeamsEndpoints(this RouteGroupBuilder group)
    {
        group.MapPost("", async (
            CreateTeamDto dto,
            ITeamCommandService teamCommandService) =>
        {
            var result = await teamCommandService.CreateTeamAsync(dto);
            return TypedResults.Ok(result);
        });

        group.MapGet("", async (
            ITeamQueryService teamQueryService) =>
        {
            var result = await teamQueryService.GetUserTeamsAsync();
            return TypedResults.Ok(result);
        });
        // TODO move to invitations endpoints
        group.MapPost("{teamId:guid}/invitations/accept", async (
            Guid teamId,
            ITeamCommandService teamCommandService) =>
        {
            await teamCommandService.AcceptInvitationAsync(new TeamId(teamId));
            return TypedResults.NoContent();
        });

        group.MapPost("{teamId:guid}/invitations/deny", async (
            Guid teamId,
            ITeamCommandService teamCommandService) =>
        {
            await teamCommandService.DenyInvitationAsync(new TeamId(teamId));
            return TypedResults.NoContent();
        });

        group.MapGet("invitations", async (
            ITeamQueryService teamQueryService) =>
        {
            var result = await teamQueryService.GetUserInvitationsAsync();
            return TypedResults.Ok(result);
        });

        return group;
    }
}
