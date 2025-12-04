using Shared.Abstractions.ValueObjects;
using Teams.Shared.Dtos;

namespace Teams.Shared.Services;

public interface ITeamMembershipQueryService
{
    Task<TeamMembershipDto?> GetMembershipAsync(UserId userId, TeamId teamId);
}