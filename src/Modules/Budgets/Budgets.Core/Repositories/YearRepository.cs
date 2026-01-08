using Dapper;
using Microsoft.Data.SqlClient;
using Budgets.Core.Repositories.Models;
using Budgets.Domain.Entities;
using Budgets.Domain.Enums;
using Budgets.Domain.ValueObjects;
using Shared.Abstractions.Factories;

namespace Budgets.Core.Repositories;

public class YearRepository(IDbConnectionFactory factory) : IYearRepository
{
    public async Task<BudgetYear?> GetByIdAsync(YearId yearId)
    {
        await using var dbConnection = new SqlConnection(factory.GetDefault());
        await dbConnection.OpenAsync();

        const string sql = """
                           SELECT Id, Value, Status, BudgetId
                           FROM [Budgets].[BudgetYears]
                           WHERE Id = @YearId
                           """;

        var dbRow = await dbConnection.QuerySingleOrDefaultAsync<BudgetYearDbRow>(sql, new { YearId = yearId.Value });
        if (dbRow == null) return null;

        return new BudgetYear.BudgetYearBuilder()
            .WithId(new YearId(dbRow.Id))
            .WithValue(new YearValue(dbRow.Value))
            .WithStatus((YearStatus)dbRow.Status)
            .WithBudgetId(new BudgetId(dbRow.BudgetId))
            .Build();
    }

    public async Task<BudgetYear?> GetByBudgetIdAndValueAsync(BudgetId budgetId, YearValue value)
    {
        await using var dbConnection = new SqlConnection(factory.GetDefault());
        await dbConnection.OpenAsync();

        const string sql = """
                           SELECT Id, Value, Status, BudgetId
                           FROM [Budgets].[BudgetYears]
                           WHERE BudgetId = @BudgetId AND Value = @Value
                           """;

        var dbRow = await dbConnection.QuerySingleOrDefaultAsync<BudgetYearDbRow>(sql, new 
        { 
            BudgetId = budgetId.Value,
            Value = value.Value
        });
        if (dbRow == null) return null;

        return new BudgetYear.BudgetYearBuilder()
            .WithId(new YearId(dbRow.Id))
            .WithValue(new YearValue(dbRow.Value))
            .WithStatus((YearStatus)dbRow.Status)
            .WithBudgetId(new BudgetId(dbRow.BudgetId))
            .Build();
    }

    public async Task<List<BudgetYear>> GetByBudgetIdAsync(BudgetId budgetId)
    {
        await using var dbConnection = new SqlConnection(factory.GetDefault());
        await dbConnection.OpenAsync();

        const string sql = """
                           SELECT Id, Value, Status, BudgetId
                           FROM [Budgets].[BudgetYears]
                           WHERE BudgetId = @BudgetId
                           ORDER BY Value ASC
                           """;

        var dbRows = await dbConnection.QueryAsync<BudgetYearDbRow>(sql, new { BudgetId = budgetId.Value });

        return dbRows.Select(dbRow => new BudgetYear.BudgetYearBuilder()
            .WithId(new YearId(dbRow.Id))
            .WithValue(new YearValue(dbRow.Value))
            .WithStatus((YearStatus)dbRow.Status)
            .WithBudgetId(new BudgetId(dbRow.BudgetId))
            .Build()).ToList();
    }

    public async Task SaveAsync(BudgetYear year)
    {
        await using var dbConnection = new SqlConnection(factory.GetDefault());
        await dbConnection.OpenAsync();

        const string sql = """
                           INSERT INTO [Budgets].[BudgetYears] (Id, BudgetId, Value, Status)
                           VALUES (@Id, @BudgetId, @Value, @Status)
                           """;

        await dbConnection.ExecuteAsync(sql, new
        {
            Id = year.Id.Value,
            BudgetId = year.BudgetId.Value,
            Value = year.Value.Value,
            Status = (int)year.Status
        });
    }

    public async Task UpdateAsync(BudgetYear year)
    {
        await using var dbConnection = new SqlConnection(factory.GetDefault());
        await dbConnection.OpenAsync();

        const string sql = """
                           UPDATE [Budgets].[BudgetYears]
                           SET Value = @Value, Status = @Status
                           WHERE Id = @Id
                           """;

        await dbConnection.ExecuteAsync(sql, new
        {
            Id = year.Id.Value,
            Value = year.Value.Value,
            Status = (int)year.Status
        });
    }

    public async Task DeleteAsync(YearId yearId)
    {
        await using var dbConnection = new SqlConnection(factory.GetDefault());
        await dbConnection.OpenAsync();

        const string sql = """
                           DELETE FROM [Budgets].[BudgetYears]
                           WHERE Id = @YearId
                           """;

        await dbConnection.ExecuteAsync(sql, new { YearId = yearId.Value });
    }
}
