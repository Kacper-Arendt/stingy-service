using Dapper;
using Microsoft.Data.SqlClient;
using Teams.Core.Repositories.Models;
using Teams.Domain.Entities;
using Teams.Domain.Enums;
using Teams.Domain.ValueObjects;
using Shared.Abstractions.Factories;
using Shared.Abstractions.ValueObjects;

namespace Teams.Core.Repositories;

public class TeamRepository : ITeamRepository
{
    private readonly IDbConnectionFactory _factory;

    public TeamRepository(IDbConnectionFactory factory)
    {
        _factory = factory;
    }

    public async Task<Team?> GetByIdAsync(TeamId teamId)
    {
        await using var dbConnection = new SqlConnection(_factory.GetDefault());
        await dbConnection.OpenAsync();

        const string teamSql = """
                               SELECT Id, Name, Description, CreatedAt, CreatedBy
                               FROM [Teams].[Teams]
                               WHERE Id = @TeamId
                               """;

        var teamRow = await dbConnection.QuerySingleOrDefaultAsync<TeamDbRow>(teamSql, new { TeamId = teamId.Value });
        if (teamRow == null) return null;

        const string participantsSql = """
                                       SELECT UserId, TeamId, Email, Role, Status, JoinedAt
                                       FROM [Teams].[TeamParticipants]
                                       WHERE TeamId = @TeamId
                                       """;

        var participantRows =
            await dbConnection.QueryAsync<TeamParticipantDbRow>(participantsSql, new { TeamId = teamId.Value });

        var participants = participantRows.Select(p => new TeamParticipant
        {
            UserId = new UserId(p.UserId),
            TeamId = new TeamId(p.TeamId),
            Email = new Email(p.Email),
            Role = (TeamParticipantRole)p.Role,
            Status = (TeamParticipantStatus)p.Status,
            JoinedAt = new CreatedAtDate(p.JoinedAt)
        }).ToList();

        return new Team.TeamBuilder()
            .WithId(new TeamId(teamRow.Id))
            .WithName(new TeamName(teamRow.Name))
            .WithDescription(new TeamDescription(teamRow.Description))
            .WithCreatedAt(new CreatedAtDate(teamRow.CreatedAt))
            .WithCreatedBy(new UserId(teamRow.CreatedBy))
            .WithParticipants(participants)
            .Build();
    }

    public async Task<List<Team>> GetUserTeamsAsync(UserId userId)
    {
        await using var dbConnection = new SqlConnection(_factory.GetDefault());
        await dbConnection.OpenAsync();

        const string sql = """
                           SELECT t.Id, t.Name, t.Description, t.CreatedAt, t.CreatedBy
                           FROM [Teams].[Teams] t
                           INNER JOIN [Teams].[TeamParticipants] tp ON t.Id = tp.TeamId
                           WHERE tp.UserId = @UserId AND tp.Status = @ActiveStatus
                           ORDER BY t.CreatedAt DESC
                           """;

        var teamRows = await dbConnection.QueryAsync<TeamDbRow>(sql, new
        {
            UserId = userId.Value,
            ActiveStatus = (int)TeamParticipantStatus.Active
        });

        var teams = new List<Team>();
        foreach (var teamRow in teamRows)
        {
            var team = await GetByIdAsync(new TeamId(teamRow.Id));
            if (team != null)
                teams.Add(team);
        }

        return teams;
    }

    public async Task<bool> IsParticipantAsync(UserId userId, TeamId teamId)
    {
        await using var dbConnection = new SqlConnection(_factory.GetDefault());

        const string sql = """
                           SELECT 1
                           FROM [Teams].[TeamParticipants]
                           WHERE UserId = @UserId AND TeamId = @TeamId AND Status = @ActiveStatus
                           """;

        var result = await dbConnection.ExecuteScalarAsync<int?>(sql, new
        {
            UserId = userId.Value,
            TeamId = teamId.Value,
            ActiveStatus = (int)TeamParticipantStatus.Active
        });

        return result.HasValue;
    }

    public async Task SaveAsync(Team team)
    {
        await using var dbConnection = new SqlConnection(_factory.GetDefault());
        await dbConnection.OpenAsync();
        await using var transaction = dbConnection.BeginTransaction();

        const string teamSql = """
                               INSERT INTO [Teams].[Teams] (Id, Name, Description, CreatedAt, CreatedBy)
                               VALUES (@Id, @Name, @Description, @CreatedAt, @CreatedBy)
                               """;

        await dbConnection.ExecuteAsync(teamSql, new
        {
            Id = team.Id.Value,
            Name = team.Name.Value,
            Description = team.Description.Value,
            CreatedAt = team.CreatedAt.Value,
            CreatedBy = team.CreatedBy.Value
        }, transaction);

        if (team.Participants.Any())
        {
            const string participantSql = """
                                          INSERT INTO [Teams].[TeamParticipants] (UserId, TeamId, Email, Role, Status, JoinedAt)
                                          VALUES (@UserId, @TeamId, @Email, @Role, @Status, @JoinedAt)
                                          """;

            var participantsToInsert = team.Participants.Select(p => new
            {
                UserId = p.UserId.Value,
                TeamId = p.TeamId.Value,
                Email = p.Email.Value,
                Role = (int)p.Role,
                Status = (int)p.Status,
                JoinedAt = p.JoinedAt.Value
            });

            await dbConnection.ExecuteAsync(participantSql, participantsToInsert, transaction);
        }

        transaction.Commit();
    }

    public async Task UpdateAsync(Team team)
    {
        await using var dbConnection = new SqlConnection(_factory.GetDefault());

        const string sql = """
                           UPDATE [Teams].[Teams]
                           SET Name = @Name, Description = @Description
                           WHERE Id = @Id
                           """;

        await dbConnection.ExecuteAsync(sql, new
        {
            Id = team.Id.Value,
            Name = team.Name.Value,
            Description = team.Description.Value
        });
    }

    public async Task DeleteAsync(TeamId teamId)
    {
        await using var dbConnection = new SqlConnection(_factory.GetDefault());

        const string sql = """
                           DELETE FROM [Teams].[Teams]
                           WHERE Id = @TeamId
                           """;

        await dbConnection.ExecuteAsync(sql, new { TeamId = teamId.Value });
    }

    public async Task AddParticipantAsync(TeamId teamId, TeamParticipant participant)
    {
        await using var dbConnection = new SqlConnection(_factory.GetDefault());

        const string sql = """
                           INSERT INTO [Teams].[TeamParticipants] (UserId, TeamId, Email, Role, Status, JoinedAt)
                           VALUES (@UserId, @TeamId, @Email, @Role, @Status, @JoinedAt)
                           """;

        await dbConnection.ExecuteAsync(sql, new
        {
            UserId = participant.UserId.Value,
            TeamId = participant.TeamId.Value,
            Email = participant.Email.Value,
            Role = (int)participant.Role,
            Status = (int)participant.Status,
            JoinedAt = participant.JoinedAt.Value
        });
    }

    public async Task RemoveParticipantAsync(TeamId teamId, UserId userId)
    {
        await using var dbConnection = new SqlConnection(_factory.GetDefault());

        const string sql = """
                           DELETE FROM [Teams].[TeamParticipants]
                           WHERE TeamId = @TeamId AND UserId = @UserId
                           """;

        await dbConnection.ExecuteAsync(sql, new
        {
            TeamId = teamId.Value,
            UserId = userId.Value
        });
    }

    public async Task<List<TeamParticipant>> GetParticipantsAsync(TeamId teamId)
    {
        await using var dbConnection = new SqlConnection(_factory.GetDefault());

        const string sql = """
                           SELECT UserId, TeamId, Email, Role, Status, JoinedAt
                           FROM [Teams].[TeamParticipants]
                           WHERE TeamId = @TeamId
                           """;

        var participantRows = await dbConnection.QueryAsync<TeamParticipantDbRow>(sql, new { TeamId = teamId.Value });

        return participantRows.Select(p => new TeamParticipant
        {
            UserId = new UserId(p.UserId),
            TeamId = new TeamId(p.TeamId),
            Email = new Email(p.Email),
            Role = (TeamParticipantRole)p.Role,
            Status = (TeamParticipantStatus)p.Status,
            JoinedAt = new CreatedAtDate(p.JoinedAt)
        }).ToList();
    }

    public async Task<List<TeamInvitationDbRow>> GetUserInvitationsAsync(Email email)
    {
        await using var dbConnection = new SqlConnection(_factory.GetDefault());

        const string sql = """
                           SELECT tp.TeamId, t.Name AS TeamName, tp.Email, tp.Role, tp.JoinedAt
                           FROM [Teams].[TeamParticipants] tp
                           INNER JOIN [Teams].[Teams] t ON t.Id = tp.TeamId
                           WHERE tp.Email = @Email AND tp.Status = @InvitedStatus
                           ORDER BY tp.JoinedAt DESC
                           """;

        var rows = await dbConnection.QueryAsync<TeamInvitationDbRow>(sql, new
        {
            Email = email.Value,
            InvitedStatus = (int)TeamParticipantStatus.Invited
        });

        return rows.ToList();
    }

    public async Task AcceptInvitationAsync(TeamId teamId, UserId userId, Email email)
    {
        await using var dbConnection = new SqlConnection(_factory.GetDefault());
        await dbConnection.OpenAsync();
        await using var transaction = dbConnection.BeginTransaction();

        // Ensure there is an invited row for this email and team
        const string existsSql = """
                                 SELECT TOP 1 1
                                 FROM [Teams].[TeamParticipants]
                                 WHERE TeamId = @TeamId AND Email = @Email AND Status = @InvitedStatus
                                 """;

        var exists = await dbConnection.ExecuteScalarAsync<int?>(existsSql, new
        {
            TeamId = teamId.Value,
            Email = email.Value,
            InvitedStatus = (int)TeamParticipantStatus.Invited
        }, transaction);

        if (!exists.HasValue)
        {
            transaction.Rollback();
            throw new UnauthorizedAccessException("No pending invitation for this user.");
        }

        // Accept invitation: set real UserId and set Status=Active
        const string updateSql = """
                                 UPDATE [Teams].[TeamParticipants]
                                 SET UserId = @UserId, Status = @ActiveStatus
                                 WHERE TeamId = @TeamId AND Email = @Email AND Status = @InvitedStatus
                                 """;

        await dbConnection.ExecuteAsync(updateSql, new
        {
            TeamId = teamId.Value,
            UserId = userId.Value,
            Email = email.Value,
            ActiveStatus = (int)TeamParticipantStatus.Active,
            InvitedStatus = (int)TeamParticipantStatus.Invited
        }, transaction);

        transaction.Commit();
    }

    public async Task DenyInvitationAsync(TeamId teamId, Email email)
    {
        await using var dbConnection = new SqlConnection(_factory.GetDefault());

        const string sql = """
                           DELETE FROM [Teams].[TeamParticipants]
                           WHERE TeamId = @TeamId AND Email = @Email AND Status = @InvitedStatus
                           """;

        await dbConnection.ExecuteAsync(sql, new
        {
            TeamId = teamId.Value,
            Email = email.Value,
            InvitedStatus = (int)TeamParticipantStatus.Invited
        });
    }

    public async Task<Dictionary<TeamId, int>> GetMemberCountsAsync(List<TeamId> teamIds)
    {
        if (!teamIds.Any()) return new Dictionary<TeamId, int>();

        await using var dbConnection = new SqlConnection(_factory.GetDefault());

        var teamGuids = teamIds.Select(id => id.Value).ToList();
        var inClause = string.Join(",", teamGuids.Select((_, i) => $"@teamId{i}"));

        var sql = $"""
                   SELECT TeamId, COUNT(*) as MemberCount
                   FROM [Teams].[TeamParticipants]
                   WHERE TeamId IN ({inClause}) AND Status = @ActiveStatus
                   GROUP BY TeamId
                   """;

        var parameters = new DynamicParameters();
        parameters.Add("@ActiveStatus", (int)TeamParticipantStatus.Active);

        for (int i = 0; i < teamGuids.Count; i++)
        {
            parameters.Add($"@teamId{i}", teamGuids[i]);
        }

        var results = await dbConnection.QueryAsync<TeamStatsDbRow>(sql, parameters);

        return results.ToDictionary(
            r => new TeamId(r.TeamId),
            r => r.MemberCount
        );
    }

    public async Task<Dictionary<TeamId, string>> GetUserRolesAsync(List<TeamId> teamIds, UserId userId)
    {
        if (!teamIds.Any()) return new Dictionary<TeamId, string>();

        await using var dbConnection = new SqlConnection(_factory.GetDefault());

        var teamGuids = teamIds.Select(id => id.Value).ToList();
        var inClause = string.Join(",", teamGuids.Select((_, i) => $"@teamId{i}"));

        var sql = $"""
                   SELECT TeamId, Role as UserRole
                   FROM [Teams].[TeamParticipants]
                   WHERE TeamId IN ({inClause}) AND UserId = @UserId
                   """;

        var parameters = new DynamicParameters();
        parameters.Add("@UserId", userId.Value);

        for (int i = 0; i < teamGuids.Count; i++)
        {
            parameters.Add($"@teamId{i}", teamGuids[i]);
        }

        var results = await dbConnection.QueryAsync<TeamStatsDbRow>(sql, parameters);

        return results.ToDictionary(
            r => new TeamId(r.TeamId),
            r => ((TeamParticipantRole)r.UserRole).ToString()
        );
    }

    public async Task<string> GetUserRoleAsync(TeamId teamId, UserId userId)
    {
        await using var dbConnection = new SqlConnection(_factory.GetDefault());

        const string sql = """
                           SELECT Role
                           FROM [Teams].[TeamParticipants]
                           WHERE TeamId = @TeamId AND UserId = @UserId
                           """;

        var result = await dbConnection.ExecuteScalarAsync<int?>(sql, new
        {
            TeamId = teamId.Value,
            UserId = userId.Value
        });

        if (result == null)
            return "Unknown";

        return ((TeamParticipantRole)result.Value).ToString();
    }
} 