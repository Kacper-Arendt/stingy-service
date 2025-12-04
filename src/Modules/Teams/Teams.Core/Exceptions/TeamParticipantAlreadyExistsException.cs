using Shared.Abstractions.Exceptions;

namespace Teams.Core.Exceptions;

public class TeamParticipantAlreadyExistsException(string email)
    : CustomException($"User with email '{email}' is already a member of this team or has a pending invitation."); 