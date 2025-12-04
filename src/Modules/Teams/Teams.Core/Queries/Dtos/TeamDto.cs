using Teams.Domain.Entities;

namespace Teams.Core.Queries.Dtos;

public record TeamDto(
    Guid Id,
    string Name,
    string Description,
    DateTime CreatedAt,
    Guid CreatedBy,
    int MemberCount,
    int RetroCount,
    DateTime? LastRetroDate,
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
            0, // Will be populated by repository
            0, // Will be populated by repository
            null, // Will be populated by repository
            string.Empty // Will be populated by repository
        );
    }

    public static TeamDto FromDomain(Team team, int memberCount, int retroCount, DateTime? lastRetroDate,
        string userRole)
    {
        return new TeamDto(
            team.Id.Value,
            team.Name.Value,
            team.Description.Value,
            team.CreatedAt.Value,
            team.CreatedBy.Value,
            memberCount,
            retroCount,
            lastRetroDate,
            userRole
        );
    }
} 