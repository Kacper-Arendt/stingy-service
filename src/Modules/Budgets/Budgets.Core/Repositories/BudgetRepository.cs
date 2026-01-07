using Dapper;
using Microsoft.Data.SqlClient;
using Budgets.Core.Repositories.Models;
using Budgets.Domain.Entities;
using Budgets.Domain.Enums;
using Budgets.Domain.ValueObjects;
using Shared.Abstractions.Factories;
using Shared.Abstractions.ValueObjects;

namespace Budgets.Core.Repositories;

public class BudgetRepository : IBudgetRepository
{
    private readonly IDbConnectionFactory _factory;

    public BudgetRepository(IDbConnectionFactory factory)
    {
        _factory = factory;
    }

    public async Task<Budget?> GetByIdAsync(BudgetId budgetId)
    {
        await using var dbConnection = new SqlConnection(_factory.GetDefault());
        await dbConnection.OpenAsync();

        const string budgetSql = """
                                 SELECT Id, Name, Description, CreatedAt
                                 FROM [Budgets].[Budgets]
                                 WHERE Id = @BudgetId
                                 """;

        var budgetRow = await dbConnection.QuerySingleOrDefaultAsync<BudgetDbRow>(budgetSql, new { BudgetId = budgetId.Value });
        if (budgetRow == null) return null;

        const string membersSql = """
                                  SELECT BudgetId, UserId, Email, Role, Status, JoinedAt
                                  FROM [Budgets].[BudgetMembers]
                                  WHERE BudgetId = @BudgetId
                                  """;

        var memberRows = await dbConnection.QueryAsync<BudgetMemberDbRow>(membersSql, new { BudgetId = budgetId.Value });

        var members = memberRows.Select(m => new BudgetMember.BudgetMemberBuilder()
            .WithBudgetId(new BudgetId(m.BudgetId))
            .WithUserId(new UserId(m.UserId))
            .WithEmail(new Email(m.Email))
            .WithRole((BudgetMemberRole)m.Role)
            .WithStatus((BudgetMemberStatus)m.Status)
            .WithJoinedAt(new CreatedAtDate(m.JoinedAt))
            .Build()).ToList();

        var budget = new Budget.BudgetBuilder()
            .WithId(new BudgetId(budgetRow.Id))
            .WithName(new BudgetName(budgetRow.Name))
            .WithDescription(new BudgetDescription(budgetRow.Description ?? string.Empty))
            .WithCreatedAt(new CreatedAtDate(budgetRow.CreatedAt))
            .Build();

        // Add members directly to the list to avoid validation errors
        foreach (var member in members)
        {
            budget.Members.Add(member);
        }

        return budget;
    }

    public async Task<List<Budget>> GetByUserIdAsync(UserId userId)
    {
        await using var dbConnection = new SqlConnection(_factory.GetDefault());
        await dbConnection.OpenAsync();

        const string sql = """
                           SELECT DISTINCT b.Id, b.Name, b.Description, b.CreatedAt
                           FROM [Budgets].[Budgets] b
                           INNER JOIN [Budgets].[BudgetMembers] bm ON b.Id = bm.BudgetId
                           WHERE bm.UserId = @UserId AND bm.Status = @ActiveStatus
                           ORDER BY b.CreatedAt DESC
                           """;

        var budgetRows = await dbConnection.QueryAsync<BudgetDbRow>(sql, new
        {
            UserId = userId.Value,
            ActiveStatus = (int)BudgetMemberStatus.Active
        });

        var budgets = new List<Budget>();
        foreach (var budgetRow in budgetRows)
        {
            var budget = await GetByIdAsync(new BudgetId(budgetRow.Id));
            if (budget != null)
                budgets.Add(budget);
        }

        return budgets;
    }

    public async Task SaveAsync(Budget budget)
    {
        await using var dbConnection = new SqlConnection(_factory.GetDefault());
        await dbConnection.OpenAsync();
        await using var transaction = dbConnection.BeginTransaction();

        const string budgetSql = """
                                 INSERT INTO [Budgets].[Budgets] (Id, Name, Description, CreatedAt)
                                 VALUES (@Id, @Name, @Description, @CreatedAt)
                                 """;

        await dbConnection.ExecuteAsync(budgetSql, new
        {
            Id = budget.Id.Value,
            Name = budget.Name.Value,
            Description = budget.Description.Value,
            CreatedAt = budget.CreatedAt.Value
        }, transaction);

        if (budget.Members.Any())
        {
            const string memberSql = """
                                     INSERT INTO [Budgets].[BudgetMembers] (BudgetId, UserId, Email, Role, Status, JoinedAt)
                                     VALUES (@BudgetId, @UserId, @Email, @Role, @Status, @JoinedAt)
                                     """;

            var membersToInsert = budget.Members.Select(m => new
            {
                BudgetId = m.BudgetId.Value,
                UserId = m.UserId.Value,
                Email = m.Email.Value,
                Role = (int)m.Role,
                Status = (int)m.Status,
                JoinedAt = m.JoinedAt.Value
            });

            await dbConnection.ExecuteAsync(memberSql, membersToInsert, transaction);
        }

        transaction.Commit();
    }

    public async Task AddMemberAsync(BudgetId budgetId, BudgetMember member)
    {
        await using var dbConnection = new SqlConnection(_factory.GetDefault());

        const string sql = """
                           INSERT INTO [Budgets].[BudgetMembers] (BudgetId, UserId, Email, Role, Status, JoinedAt)
                           VALUES (@BudgetId, @UserId, @Email, @Role, @Status, @JoinedAt)
                           """;

        await dbConnection.ExecuteAsync(sql, new
        {
            BudgetId = budgetId.Value,
            UserId = member.UserId.Value,
            Email = member.Email.Value,
            Role = (int)member.Role,
            Status = (int)member.Status,
            JoinedAt = member.JoinedAt.Value
        });
    }

    public async Task RemoveMemberAsync(BudgetId budgetId, UserId userId)
    {
        await using var dbConnection = new SqlConnection(_factory.GetDefault());

        const string sql = """
                           DELETE FROM [Budgets].[BudgetMembers]
                           WHERE BudgetId = @BudgetId AND UserId = @UserId
                           """;

        await dbConnection.ExecuteAsync(sql, new
        {
            BudgetId = budgetId.Value,
            UserId = userId.Value
        });
    }

    public async Task<bool> IsMemberAsync(UserId userId, BudgetId budgetId)
    {
        await using var dbConnection = new SqlConnection(_factory.GetDefault());

        const string sql = """
                           SELECT 1
                           FROM [Budgets].[BudgetMembers]
                           WHERE UserId = @UserId AND BudgetId = @BudgetId AND Status = @ActiveStatus
                           """;

        var result = await dbConnection.ExecuteScalarAsync<int?>(sql, new
        {
            UserId = userId.Value,
            BudgetId = budgetId.Value,
            ActiveStatus = (int)BudgetMemberStatus.Active
        });

        return result.HasValue;
    }
}
