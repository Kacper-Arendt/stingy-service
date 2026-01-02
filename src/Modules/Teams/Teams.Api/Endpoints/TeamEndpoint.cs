using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Shared.Abstractions.ValueObjects;
using Teams.Core.Authorization;
using Teams.Core.Commands;
using Teams.Core.Commands.Dtos;
using Teams.Core.Queries;

namespace Teams.Api.Endpoints;

public static class TeamEndpoint
{
    public static RouteGroupBuilder MapTeamEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("{teamId:guid}", async (
            Guid teamId,
            ITeamQueryService teamQueryService) =>
        {
            var result = await teamQueryService.GetByIdAsync(new TeamId(teamId));
            return result is null ? Results.NotFound() : TypedResults.Ok(result);
        }).RequireAuthorization(Policies.HasAccessToTeam);

        group.MapPut("{teamId:guid}", async (
            Guid teamId,
            UpdateTeamDto dto,
            ITeamCommandService teamCommandService) =>
        {
            var result = await teamCommandService.UpdateTeamAsync(new TeamId(teamId), dto);
            return TypedResults.Ok(result);
        }).RequireAuthorization(Policies.HasAccessToTeam);
        
        group.MapDelete("{teamId:guid}", async (
            Guid teamId,
            ITeamCommandService teamCommandService) =>
        {
            await teamCommandService.DeleteTeamAsync(new TeamId(teamId));
            return TypedResults.NoContent();
        }).RequireAuthorization(Policies.HasAccessToTeam);

        group.MapGet("{teamId:guid}/participants", async (
            Guid teamId,
            ITeamQueryService teamQueryService) =>
        {
            var result = await teamQueryService.GetParticipantsAsync(new TeamId(teamId));
            return TypedResults.Ok(result);
        }).RequireAuthorization(Policies.HasAccessToTeam);

        group.MapPost("{teamId:guid}/participants", async (
            Guid teamId,
            AddTeamParticipantDto dto,
            ITeamCommandService teamCommandService) =>
        {
            await teamCommandService.AddParticipantAsync(new TeamId(teamId), dto);
            return TypedResults.Ok();
        }).RequireAuthorization(Policies.HasAccessToTeam);

        group.MapDelete("{teamId:guid}/participants/{userId:guid}", async (
            Guid teamId,
            Guid userId,
            ITeamCommandService teamCommandService) =>
        {
            await teamCommandService.RemoveParticipantAsync(new TeamId(teamId), new(userId));
            return TypedResults.NoContent();
        }).RequireAuthorization(Policies.HasAccessToTeam);
        
        return group;
    }
}