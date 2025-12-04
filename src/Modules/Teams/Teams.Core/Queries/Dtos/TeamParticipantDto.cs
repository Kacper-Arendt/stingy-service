using Teams.Domain.Entities;

namespace Teams.Core.Queries.Dtos;

public record TeamParticipantDto(
    Guid UserId,
    Guid TeamId,
    AuthorDto Author,
    string Email,
    string Role,
    string Status,
    DateTime JoinedAt)
{
    public static TeamParticipantDto FromDomain(TeamParticipant participant, AuthorDto author)
    {
        return new TeamParticipantDto(
            participant.UserId.Value,
            participant.TeamId.Value,
            author,
            participant.Email.Value,
            participant.Role.ToString(),
            participant.Status.ToString(),
            participant.JoinedAt.Value
        );
    }
} 