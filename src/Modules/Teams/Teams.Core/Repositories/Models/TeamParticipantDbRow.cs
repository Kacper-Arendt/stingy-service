namespace Teams.Core.Repositories.Models;

public class TeamParticipantDbRow
{
    public Guid UserId { get; set; }
    public Guid TeamId { get; set; }
    public string Email { get; set; } = string.Empty;
    public int Role { get; set; }
    public int Status { get; set; }
    public DateTime JoinedAt { get; set; }
} 