namespace Teams.Core.Queries.Dtos;

public class TeamPermissions
{
    public bool CanEditTeam { get; set; }
    public bool CanDeleteTeam { get; set; }
    public bool CanManageParticipants { get; set; }
    public bool CanChangeUserRoles { get; set; }
    public bool CanCreateRetro { get; set; }
    public bool CanInviteUsers { get; set; }
} 