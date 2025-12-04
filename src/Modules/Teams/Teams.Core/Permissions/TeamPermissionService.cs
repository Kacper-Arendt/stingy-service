using Microsoft.Extensions.Caching.Memory;
using Teams.Core.Queries.Dtos;
using Teams.Domain.Entities;
using Teams.Domain.Enums;
using Shared.Abstractions.ValueObjects;

namespace Teams.Core.Permissions;

public class TeamPermissionService : ITeamPermissionService
{
    private static readonly MemoryCache PermissionsCache = new(new MemoryCacheOptions());
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(1);

    public bool CanUpdateTeam(UserId userId, Team team)
    {
        if (!team.IsParticipant(userId)) return false;
        return team.IsAdmin(userId);
    }

    public bool CanDeleteTeam(UserId userId, Team team)
    {
        if (!team.IsParticipant(userId)) return false;
        return team.IsOwner(userId);
    }

    public bool CanManageParticipants(UserId userId, Team team)
    {
        if (!team.IsParticipant(userId)) return false;
        return team.IsAdmin(userId);
    }

    public bool CanViewTeam(UserId userId, Team team)
    {
        return team.IsParticipant(userId);
    }

    public bool CanRemoveParticipant(UserId currentUserId, Team team, UserId targetUserId)
    {
        if (!team.IsParticipant(currentUserId)) return false;

        // User can remove themselves
        if (currentUserId.Value == targetUserId.Value) return true;

        // Only admins can remove other participants
        if (!team.IsAdmin(currentUserId)) return false;

        var targetParticipant = team.GetParticipant(targetUserId);
        var currentParticipant = team.GetParticipant(currentUserId);

        // Owner can remove anyone
        if (currentParticipant.Role == TeamParticipantRole.Owner) return true;

        // Admin can remove members but not other admins or owners
        if (currentParticipant.Role == TeamParticipantRole.Admin)
        {
            return targetParticipant.Role == TeamParticipantRole.Member;
        }

        return false;
    }

    public TeamPermissions GetUserPermissions(UserId userId, Team team)
    {
        var cacheKey = $"team_permissions:{userId.Value}:{team.Id.Value}";

        if (PermissionsCache.TryGetValue(cacheKey, out TeamPermissions? cachedPermissions))
        {
            return cachedPermissions!;
        }

        var permissions = new TeamPermissions
        {
            CanEditTeam = CanUpdateTeam(userId, team),
            CanDeleteTeam = CanDeleteTeam(userId, team),
            CanManageParticipants = CanManageParticipants(userId, team),
            CanChangeUserRoles = CanChangeUserRoles(userId, team),
            CanCreateRetro = CanCreateRetro(userId, team),
            CanInviteUsers = CanInviteUsers(userId, team)
        };

        PermissionsCache.Set(cacheKey, permissions, CacheDuration);
        return permissions;
    }

    public bool CanCreateRetro(UserId userId, Team team)
    {
        // Only team admins (Admins and Owners) can create retrospectives
        if (!team.IsParticipant(userId)) return false;
        return team.IsAdmin(userId);
    }

    public bool CanChangeUserRoles(UserId userId, Team team)
    {
        if (!team.IsParticipant(userId)) return false;
        return team.IsOwner(userId); // Only owners can change roles
    }

    public bool CanInviteUsers(UserId userId, Team team)
    {
        if (!team.IsParticipant(userId)) return false;
        return team.IsAdmin(userId); // Admins and Owners can invite users
    }
} 