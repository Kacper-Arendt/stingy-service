using Teams.Core.Queries.Dtos;
using Teams.Domain.Entities;
using Teams.Domain.ValueObjects;
using Shared.Abstractions.ValueObjects;

namespace Teams.Core.Permissions;

public interface ITeamPermissionService
{
    bool CanUpdateTeam(UserId userId, Team team);
    bool CanDeleteTeam(UserId userId, Team team);
    bool CanManageParticipants(UserId userId, Team team);
    bool CanViewTeam(UserId userId, Team team);
    bool CanRemoveParticipant(UserId currentUserId, Team team, UserId targetUserId);
    
    // New methods for DTO generation
    TeamPermissions GetUserPermissions(UserId userId, Team team);
    bool CanCreateRetro(UserId userId, Team team);
    bool CanChangeUserRoles(UserId userId, Team team);
    bool CanInviteUsers(UserId userId, Team team);
} 