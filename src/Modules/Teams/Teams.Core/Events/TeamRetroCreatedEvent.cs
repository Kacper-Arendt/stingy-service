using Retro.Domain.ValueObjects;
using Teams.Domain.ValueObjects;
using Shared.Abstractions.Events;
using Shared.Abstractions.ValueObjects;

namespace Teams.Core.Events;

// TODO Fix this 
public class TeamRetroCreatedEvent(
    TeamId teamId,
    TeamName teamName,
    Email inviterEmail,
    List<Email> invitedEmails,
    RetroName retroName)
    : IEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;

    public TeamId TeamId { get; set; } = teamId;
    public TeamName TeamName { get; set; } = teamName;
    public Email InviterEmail { get; set; } = inviterEmail;
    public List<Email> InvitedEmails { get; set; } = invitedEmails;
    public RetroName RetroName { get; set; } = retroName;
} 