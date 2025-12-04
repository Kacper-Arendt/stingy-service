namespace Teams.Core.Queries.Dtos;

public record TeamUserInvitationDto(
    Guid TeamId,
    string TeamName,
    string Email,
    string Role,
    DateTime InvitedAt);


