using Shared.Abstractions.ValueObjects;
using Shared.Infrastructure.Helpers;
using Teams.Core.Permissions;
using Teams.Core.Queries.Dtos;
using Teams.Core.Repositories;
using Teams.Domain.Enums;
using UserProfiles.Shared.Dtos;
using UserProfiles.Shared.Services;

namespace Teams.Core.Queries;

public class TeamQueryService : ITeamQueryService
{
    private readonly ITeamRepository _teamRepository;
    private readonly HttpContextHelper _httpContextHelper;
    private readonly ITeamPermissionService _permissionService;
    private readonly IUserDisplayService _userDisplayService;

    public TeamQueryService(ITeamRepository teamRepository, HttpContextHelper httpContextHelper,
        ITeamPermissionService permissionService, IUserDisplayService userDisplayService)
    {
        _teamRepository = teamRepository;
        _httpContextHelper = httpContextHelper;
        _permissionService = permissionService;
        _userDisplayService = userDisplayService;
    }

    public async Task<TeamDetailsDto?> GetByIdAsync(TeamId teamId)
    {
        var user = _httpContextHelper.GetCurrentUser();
        var team = await _teamRepository.GetByIdAsync(teamId);

        if (team == null) return null;

        if (!team.IsParticipant(user.Id))
            throw new UnauthorizedAccessException("You are not a member of this team.");

        var userRole = await _teamRepository.GetUserRoleAsync(teamId, user.Id);
        var permissions = _permissionService.GetUserPermissions(user.Id, team);

        // Get author info for all participants
        var participantIds = team.Participants.Select(p => p.UserId).Distinct().ToList();
        var authorsDisplayInfo = await _userDisplayService.GetUsersDisplayInfoAsync(participantIds);
        var authorsDict = authorsDisplayInfo.ToDictionary(a => a.UserId, a => AuthorDto.FromUserDisplayInfo(a));

        var participantsWithAuthors = team.Participants.Select(participant =>
        {
            var author = authorsDict.GetValueOrDefault(participant.UserId.Value) 
                ?? new AuthorDto { UserId = participant.UserId.Value, DisplayName = "Unknown User", ShortDisplayName = "??", ProfileImageUrl = null };
            
            return TeamParticipantDto.FromDomain(participant, author);
        }).ToList();

        return TeamDetailsDto.FromDomain(team, userRole, permissions, participantsWithAuthors);
    }

    public async Task<List<TeamDto>> GetUserTeamsAsync()
    {
        var user = _httpContextHelper.GetCurrentUser();
        var teams = await _teamRepository.GetUserTeamsAsync(user.Id);

        if (teams.Count == 0) return [];

        var teamIds = teams.Select(t => t.Id).ToList();
        var memberCounts = await _teamRepository.GetMemberCountsAsync(teamIds);
        var retroCounts = await _teamRepository.GetRetroCountsAsync(teamIds);
        var lastRetroDates = await _teamRepository.GetLastRetroDateAsync(teamIds);
        var userRoles = await _teamRepository.GetUserRolesAsync(teamIds, user.Id);

        return teams.Select(team => TeamDto.FromDomain(
            team,
            memberCounts.GetValueOrDefault(team.Id, 0),
            retroCounts.GetValueOrDefault(team.Id, 0),
            lastRetroDates.GetValueOrDefault(team.Id, null),
            userRoles.GetValueOrDefault(team.Id, "Unknown")
        )).ToList();
    }

    public async Task<List<TeamParticipantDto>> GetParticipantsAsync(TeamId teamId)
    {
        var user = _httpContextHelper.GetCurrentUser();
        var team = await _teamRepository.GetByIdAsync(teamId);

        if (team == null)
            throw new InvalidOperationException("Team not found.");

        if (!team.IsParticipant(user.Id))
            throw new UnauthorizedAccessException("You are not a member of this team.");

        var participants = await _teamRepository.GetParticipantsAsync(teamId);
        
        // Get author info for all participants
        var participantIds = participants.Select(p => p.UserId).Distinct().ToList();
        var authorsDisplayInfo = await _userDisplayService.GetUsersDisplayInfoAsync(participantIds);
        var authorsDict = authorsDisplayInfo.ToDictionary(a => a.UserId, a => AuthorDto.FromUserDisplayInfo(a));

        return participants.Select(participant =>
        {
            var defName = participant.Email ?? "Unknown User";
            var author = authorsDict.GetValueOrDefault(participant.UserId.Value) 
                ?? new AuthorDto { UserId = participant.UserId.Value, DisplayName = defName, ShortDisplayName = UserDisplayInfoDto.GenerateShortDisplayName(defName), ProfileImageUrl = null };
            
            return TeamParticipantDto.FromDomain(participant, author);
        }).ToList();
    }

    public async Task<bool> IsParticipantAsync(TeamId teamId, UserId userId)
    {
        return await _teamRepository.IsParticipantAsync(userId, teamId);
    }

    public async Task<List<TeamUserInvitationDto>> GetUserInvitationsAsync()
    {
        var user = _httpContextHelper.GetCurrentUser();
        var rows = await _teamRepository.GetUserInvitationsAsync(user.Email);
        if (!rows.Any()) return [];

        return rows.Select(r => new TeamUserInvitationDto(
            r.TeamId,
            r.TeamName,
            r.Email,
            ((TeamParticipantRole)r.Role).ToString(),
            r.JoinedAt
        )).ToList();
    }
} 