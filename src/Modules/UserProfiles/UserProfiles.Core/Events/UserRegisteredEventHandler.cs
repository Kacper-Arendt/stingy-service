using Shared.Abstractions.Events;
using UserProfiles.Core.Commands;
using Microsoft.Extensions.Logging;

namespace UserProfiles.Core.Events;

public class UserRegisteredEventHandler : IEventHandler<UserRegisteredEvent>
{
    private readonly IUserProfileCommandService _userProfileCommandService;
    private readonly ILogger<UserRegisteredEventHandler> _logger;

    public UserRegisteredEventHandler(
        IUserProfileCommandService userProfileCommandService,
        ILogger<UserRegisteredEventHandler> logger)
    {
        _userProfileCommandService = userProfileCommandService;
        _logger = logger;
    }

    public async Task HandleAsync(UserRegisteredEvent @event)
    {
        try
        {
            _logger.LogInformation("Creating profile for newly registered user {UserId}", @event.UserId.Value);
            
            await _userProfileCommandService.CreateFromEmailAsync(@event.UserId, @event.Email);
            
            _logger.LogInformation("Successfully created profile for user {UserId}", @event.UserId.Value);
        }
        catch (Exception ex)
        {
            SentrySdk.CaptureException(ex);
            _logger.LogError(ex, "Failed to create profile for user {UserId}", @event.UserId.Value);
        }
    }
}
