using Teams.Core.Commands.Dtos;
using Teams.Core.Queries.Dtos;
using Shared.Abstractions.ValueObjects;

namespace Teams.Core.Commands;

public interface ITeamCommandService
{
    Task<TeamDto> CreateTeamAsync(CreateTeamDto dto);
    Task<TeamDto> UpdateTeamAsync(TeamId teamId, UpdateTeamDto dto);
    Task DeleteTeamAsync(TeamId teamId);
    Task AddParticipantAsync(TeamId teamId, AddTeamParticipantDto dto);
    Task RemoveParticipantAsync(TeamId teamId, UserId userId);
    Task AcceptInvitationAsync(TeamId teamId);
    Task DenyInvitationAsync(TeamId teamId);
} 