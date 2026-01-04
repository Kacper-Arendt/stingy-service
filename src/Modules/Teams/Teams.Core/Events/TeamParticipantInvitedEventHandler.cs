using Shared.Abstractions.Communication;
using Shared.Abstractions.Events;

namespace Teams.Core.Events;

public class TeamParticipantInvitedEventHandler : IEventHandler<TeamParticipantInvitedEvent>
{
    private readonly IEmailSender _emailSender;

    public TeamParticipantInvitedEventHandler(IEmailSender emailSender)
    {
        _emailSender = emailSender;
    }

    // todo change app name, move to appsettings
    public async Task HandleAsync(TeamParticipantInvitedEvent @event)
    {
        var body = $"""
                    <p style='color:#555;font-size:16px;font-family:sans-serif;margin-bottom:32px;'>
                        You have been invited to join Team <strong>{@event.TeamName.Value}</strong> by {@event.InviterEmail.Value}. Click below to accept the invitation. You need to be logged in to accept the invitation.
                    </p>
                    <a href='https://app.retrospectacle.xyz/teams/{@event.TeamId.Value}' style='display:inline-block;padding:12px 28px;background-color:#4f46e5;color:#fff;text-decoration:none;border-radius:4px;font-size:16px;font-family:sans-serif;'>
                        Accept Invitation
                    </a>
                    """;

        await _emailSender.SendEmailAsync(
            [@event.InvitedEmail],
            "Invitation to join Team",
            body
        );
    }
} 