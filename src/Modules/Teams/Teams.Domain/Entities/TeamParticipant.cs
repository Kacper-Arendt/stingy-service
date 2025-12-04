using Teams.Domain.Enums;
using Teams.Domain.ValueObjects;
using Shared.Abstractions.ValueObjects;
using Retro.Domain.ValueObjects;

namespace Teams.Domain.Entities;

public class TeamParticipant
{
    public UserId UserId { get; set; } = null!;
    public TeamId TeamId { get; set; } = null!;
    public Email Email { get; set; } = null!;
    public TeamParticipantRole Role { get; set; }
    public TeamParticipantStatus Status { get; set; }
    public CreatedAtDate JoinedAt { get; set; } = null!;

    public void ChangeRole(TeamParticipantRole newRole)
    {
        if (Role == newRole)
            throw new InvalidOperationException("New role must be different from current role.");

        Role = newRole;
    }

    public void Activate()
    {
        if (Status == TeamParticipantStatus.Active)
            throw new InvalidOperationException("Participant is already active.");

        Status = TeamParticipantStatus.Active;
    }

    public void Deactivate()
    {
        if (Status == TeamParticipantStatus.Inactive)
            throw new InvalidOperationException("Participant is already inactive.");

        Status = TeamParticipantStatus.Inactive;
    }
} 