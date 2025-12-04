using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Caching.Memory;
using Shared.Abstractions.Factories;
using Shared.Abstractions.ValueObjects;
using Teams.Shared.Dtos;
using Teams.Shared.Enums;

namespace Teams.Shared.Services;

public class TeamMembershipQueryService : ITeamMembershipQueryService
{
    private readonly IDbConnectionFactory _connectionFactory;
    private static readonly MemoryCache MembershipCache = new(new MemoryCacheOptions());
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(3);

    public TeamMembershipQueryService(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<TeamMembershipDto?> GetMembershipAsync(UserId userId, TeamId teamId)
    {
        var cacheKey = $"membership:{userId.Value}:{teamId.Value}";

        if (MembershipCache.TryGetValue(cacheKey, out TeamMembershipDto? cachedMembership))
        {
            return cachedMembership;
        }

        await using var connection = new SqlConnection(_connectionFactory.GetDefault());
        await connection.OpenAsync();

        const string sql =
            """
            SELECT UserId, TeamId, Email, Role 
            FROM Teams.TeamParticipants 
            WHERE UserId = @UserId AND TeamId = @TeamId AND Status = 0
            """;

        var result = await connection.QueryFirstOrDefaultAsync<TeamParticipantDbRow>(
            sql,
            new
            {
                UserId = userId.Value,
                TeamId = teamId.Value
            });

        var membership = result == null
            ? null
            : new TeamMembershipDto(
                result.UserId,
                result.TeamId,
                result.Email,
                (TeamParticipantRole)result.Role);

        // Cache the result (including null values to avoid repeated queries for non-members)
        MembershipCache.Set(cacheKey, membership, CacheDuration);

        return membership;
    }

    private class TeamParticipantDbRow
    {
        public Guid UserId { get; set; }
        public Guid TeamId { get; set; }
        public string Email { get; set; } = string.Empty;
        public int Role { get; set; }
    }
}