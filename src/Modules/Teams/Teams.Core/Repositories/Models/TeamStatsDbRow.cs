namespace Teams.Core.Repositories.Models;

public class TeamStatsDbRow
{
    public required Guid TeamId { get; set; }
    public int MemberCount { get; set; }
    public int RetroCount { get; set; }
    public DateTime? LastRetroDate { get; set; }
    public int UserRole { get; set; }
} 