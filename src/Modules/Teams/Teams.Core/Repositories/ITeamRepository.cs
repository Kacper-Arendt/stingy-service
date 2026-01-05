using Teams.Domain.Entities;
using Teams.Domain.ValueObjects;
using Shared.Abstractions.ValueObjects;

namespace Teams.Core.Repositories;

public interface ITeamRepository
{
    Task<Team?> GetByIdAsync(TeamId teamId);
    Task<List<Team>> GetUserTeamsAsync(UserId userId);
    Task<bool> IsParticipantAsync(UserId userId, TeamId teamId);
    Task SaveAsync(Team team);
    Task UpdateAsync(Team team);
    Task DeleteAsync(TeamId teamId);
    Task AddParticipantAsync(TeamId teamId, TeamParticipant participant);
    Task RemoveParticipantAsync(TeamId teamId, UserId userId);
    Task<List<TeamParticipant>> GetParticipantsAsync(TeamId teamId);
    Task<List<Models.TeamInvitationDbRow>> GetUserInvitationsAsync(Email email);
    Task AcceptInvitationAsync(TeamId teamId, UserId userId, Email email);
    Task DenyInvitationAsync(TeamId teamId, Email email);
    Task<Dictionary<TeamId, int>> GetMemberCountsAsync(List<TeamId> teamIds);
    Task<Dictionary<TeamId, string>> GetUserRolesAsync(List<TeamId> teamIds, UserId userId);
    Task<string> GetUserRoleAsync(TeamId teamId, UserId userId);
} 