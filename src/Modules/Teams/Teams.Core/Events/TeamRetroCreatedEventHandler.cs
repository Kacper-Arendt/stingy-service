using Shared.Abstractions.Communication;
using Shared.Abstractions.Events;

namespace Teams.Core.Events;

public class TeamRetroCreatedEventHandler : IEventHandler<TeamRetroCreatedEvent>
{
    private readonly IEmailSender _emailSender;

    public TeamRetroCreatedEventHandler(IEmailSender emailSender)
    {
        _emailSender = emailSender;
    }

    public async Task HandleAsync(TeamRetroCreatedEvent @event)
    {
        var body = $"""
                    <p style='color:#555;font-size:16px;font-family:sans-serif;margin-bottom:32px;'>
                        A new retro <strong>{@event.RetroName.Value}</strong> has been created in Team <strong>{@event.TeamName.Value}</strong> by {@event.InviterEmail.Value}.
                    </p>
                    """;

        await _emailSender.SendEmailAsync(
            @event.InvitedEmails,
            "A new retro has been created in your team",
            body
        );
    }
}