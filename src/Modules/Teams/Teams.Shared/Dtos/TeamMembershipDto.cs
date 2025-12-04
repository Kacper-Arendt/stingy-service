using Teams.Shared.Enums;

namespace Teams.Shared.Dtos;

public record TeamMembershipDto(
    Guid UserId,
    Guid TeamId,
    string Email,
    TeamParticipantRole Role);