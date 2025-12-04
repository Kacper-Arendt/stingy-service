using Shared.Abstractions.ValueObjects;
using Teams.Core.Queries.Dtos;

namespace Teams.Core.Queries;

public interface ITeamQueryService
{
    Task<TeamDetailsDto?> GetByIdAsync(TeamId teamId);
    Task<List<TeamDto>> GetUserTeamsAsync();
    Task<List<TeamParticipantDto>> GetParticipantsAsync(TeamId teamId);
    Task<List<TeamUserInvitationDto>> GetUserInvitationsAsync();
    Task<bool> IsParticipantAsync(TeamId teamId, UserId userId);
} 