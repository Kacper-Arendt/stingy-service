using Teams.Domain.ValueObjects;
using Shared.Abstractions.Events;
using Shared.Abstractions.ValueObjects;

namespace Teams.Core.Events;

public class TeamParticipantInvitedEvent : IEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;

    public TeamId TeamId { get; set; }
    public TeamName TeamName { get; set; }
    public Email InviterEmail { get; set; }
    public Email InvitedEmail { get; set; }
    
    public TeamParticipantInvitedEvent(TeamId teamId, TeamName teamName, Email inviterEmail, Email invitedEmail)
    {
        TeamId = teamId;
        TeamName = teamName;
        InviterEmail = inviterEmail;
        InvitedEmail = invitedEmail;
    }
} 