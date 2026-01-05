using Teams.Domain.Entities;

namespace Teams.Core.Queries.Dtos;

public record TeamDetailsDto(
    Guid Id,
    string Name,
    string Description,
    DateTime CreatedAt,
    Guid CreatedBy,
    List<TeamParticipantDto> Participants,
    string UserRole,
    TeamPermissions Permissions)
{
    public static TeamDetailsDto FromDomain(Team team, string userRole, TeamPermissions permissions, List<TeamParticipantDto> participants)
    {
        return new TeamDetailsDto(
            team.Id.Value,
            team.Name.Value,
            team.Description.Value,
            team.CreatedAt.Value,
            team.CreatedBy.Value,
            participants,
            userRole,
            permissions);
    }
} 