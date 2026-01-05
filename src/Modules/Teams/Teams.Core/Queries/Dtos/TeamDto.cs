using Teams.Domain.Entities;

namespace Teams.Core.Queries.Dtos;

public record TeamDto(
    Guid Id,
    string Name,
    string Description,
    DateTime CreatedAt,
    Guid CreatedBy,
    int MemberCount,
    string UserRole)
{
    public static TeamDto FromDomain(Team team)
    {
        return new TeamDto(
            team.Id.Value,
            team.Name.Value,
            team.Description.Value,
            team.CreatedAt.Value,
            team.CreatedBy.Value,
            0,
            string.Empty 
        );
    }

    public static TeamDto FromDomain(Team team, int memberCount,
        string userRole)
    {
        return new TeamDto(
            team.Id.Value,
            team.Name.Value,
            team.Description.Value,
            team.CreatedAt.Value,
            team.CreatedBy.Value,
            memberCount,
            userRole
        );
    }
} 