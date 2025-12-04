namespace Teams.Core.Authorization;

public static class Policies
{
    public const string HasAccessToTeam = "HasAccessToTeam";
    public const string CanManageTeam = "CanManageTeam";
    public const string CanManageTeamParticipants = "CanManageTeamParticipants";
} 