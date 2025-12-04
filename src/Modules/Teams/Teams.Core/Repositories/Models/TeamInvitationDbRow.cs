namespace Teams.Core.Repositories.Models;

public class TeamInvitationDbRow
{
    public Guid TeamId { get; set; }
    public string TeamName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int Role { get; set; }
    public DateTime JoinedAt { get; set; }
}


